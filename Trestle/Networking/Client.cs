using System;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Trestle.Configuration.Service;
using Trestle.Networking.Attributes;
using Trestle.Networking.Enums;
using Trestle.Networking.Services;

namespace Trestle.Networking
{
    public class Client : IDisposable
    {
        public State State = State.Handshaking;

        public bool IsLocalhost
            => _tcpClient.Client.RemoteEndPoint.ToString().Contains("127.0.0.1");
        
        /// <summary>
        /// <see cref="TcpClient"/> of the Client.
        /// </summary>
        private readonly TcpClient _tcpClient;

        private readonly IMojangService _mojangService;
        private readonly IPacketService _packetService;
        private readonly IClientService _clientService;
        private readonly IConfigService _configService;

        public Client(IMojangService mojangService,  IPacketService packetService, IClientService clientService, TcpClient tcpClient, IConfigService configService)
        {
            _mojangService = mojangService;
            _clientService = clientService;
            _packetService = packetService;
            _configService = configService;
            
            _tcpClient = tcpClient;

            _ = HandlePackets();
        }
        
        #region Packets

        public void SendPacket(Packet packet)
        {
            if (packet.GetType().GetCustomAttribute<ClientBoundAttribute>(true) == null)
                throw new IOException("Unable to send a non-clientbound packet.");
            
            packet.Client = this;
            _tcpClient.Client.Send(packet.Serialize());
        }
        
        /// <summary>
        /// Continuously checks for new data, and then parses it.
        /// </summary>
        private async Task HandlePackets()
        {
            var stream = _tcpClient.GetStream();

            var b = new byte[1];
            while (_tcpClient.Client.Receive(b, SocketFlags.Peek) != 0) // Continues checking for new data, while still connected.
            {
                while (!stream.DataAvailable)
                    Thread.Sleep(5);

                try
                {
                    var packet = false // TODO: replace hardcode with compression config
                        ? _packetService.ParseCompressedPacket(this, stream) 
                        : _packetService.ParseUncompressedPacket(this, stream);

                    // Initializes all variables inside of the packet, and then calls it.
                    packet.Initialize(this, _clientService, _mojangService, _configService);
                    await packet.Handle();
                    
                    // Disposes of the packet when it is done be handled.
                    packet.Dispose();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            
            // If this is reached, then there is no longer a connection, so unregister it.
            _clientService.UnregisterClient(this);
        }
        
        #endregion
        
        #region Disposing

        public void Dispose()
        {
            _tcpClient.Dispose();
        }

        #endregion
    }
}