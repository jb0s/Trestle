using System;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Trestle.Networking.Attributes;
using Trestle.Networking.Enums;
using Trestle.Networking.Services;

namespace Trestle.Networking
{
    public class Client : IDisposable
    {
        public State State = State.Handshaking;
        
        /// <summary>
        /// <see cref="TcpClient"/> of the Client.
        /// </summary>
        private readonly TcpClient _tcpClient;

        private readonly IClientService _clientService;
        private readonly IPacketService _packetService;
        
        public Client(IClientService clientService, IPacketService packetService, TcpClient tcpClient)
        {
            _clientService = clientService;
            _packetService = packetService;
            
            _tcpClient = tcpClient;

            new Task(HandlePackets).Start();
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
        private void HandlePackets()
        {
            var stream = _tcpClient.GetStream();

            var b = new byte[1];
            while (_tcpClient.Client.Receive(b, SocketFlags.Peek) != 0) // Continues checking for new data, while still connected.
            {
                while (!stream.DataAvailable)
                    Thread.Sleep(5);
                
                var packet = false // TODO: replace hardcode with compression config
                    ? _packetService.ParseCompressedPacket(stream) 
                    : _packetService.ParseUncompressedPacket(this, stream);

                // Tells the packet the Client and then handles it.
                packet.Handle();
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