﻿using System;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Trestle.Configuration.Service;
using Trestle.Entities.Players;
using Trestle.Levels;
using Trestle.Levels.Enums;
using Trestle.Levels.Services;
using Trestle.Networking.Attributes;
using Trestle.Networking.Enums;
using Trestle.Networking.Packets.Play.Client;
using Trestle.Networking.Services;

namespace Trestle.Networking
{
    public class Client : IDisposable
    {
        /// <summary>
        /// Player that the Client is associated with.
        /// </summary>
        public Player Player { get; private set; }
        
        /// <summary>
        /// State that packets are in.
        /// </summary>
        public State State { get; set; } = State.Handshaking;

        /// <summary>
        /// The amount of KeepAlives the client has missed.
        /// The client is disconnected if this number is equal to 5.
        /// </summary>
        public int MissedKeepAlives { get; set; }
        
        /// <summary>
        /// The last time a KeepAlive was attempted in milliseconds.
        /// </summary>
        public int LastKeepAliveAttempt { get; set; }
        
        /// <summary>
        /// The last time a KeepAlive was successful in milliseconds.
        /// </summary>
        public int LastKeepAliveSuccess { get; set; }

        /// <summary>
        /// Latency from server to client.
        /// </summary>
        public int Ping
            => LastKeepAliveSuccess - LastKeepAliveAttempt;
        
        /// <summary>
        /// Is this client connected from a local machine?
        /// </summary>
        public bool IsLocalhost
            => _tcpClient.Client.RemoteEndPoint.ToString().Contains("127.0.0.1");

        /// <summary>
        /// Is there a valid connection between Client and Server?
        /// </summary>
        public bool IsConnected
            => _tcpClient.Client.Receive(new byte[1], SocketFlags.Peek) != 0;

        /// <summary>
        /// <see cref="TcpClient"/> of the Client.
        /// </summary>
        private readonly TcpClient _tcpClient;

        private readonly IMojangService _mojangService;
        private readonly IPacketService _packetService;
        private readonly IClientService _clientService;
        private readonly IConfigService _configService;
        private readonly ILevelService _levelService;

        public Client(IMojangService mojangService, IPacketService packetService, IClientService clientService, TcpClient tcpClient, IConfigService configService, ILevelService levelService)
        {
            _mojangService = mojangService;
            _clientService = clientService;
            _packetService = packetService;
            _configService = configService;
            _levelService = levelService;
            
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

            while (IsConnected) // Continues checking for new data, while still connected.
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
                { }
            }
            
            // If this is reached, then there is no longer a connection, so unregister it.
            _clientService.UnregisterClient(this);
        }
        
        /// <summary>
        /// Sends a packet to the client every 2500ms to validate the connection.
        /// </summary>
        /// <param name="keepAliveId"></param>
        /// <exception cref="NotImplementedException"></exception>
        public async Task KeepAlive()
        {
            while (State == State.Play)
            {
                await Task.Delay(2500);

                LastKeepAliveAttempt = DateTime.Now.Millisecond;
                
                // Increase the MissedKeepAlives value and send a KeepAlive packet.
                // Once a ServerBound KeepAlive packet is received, we reset MissedKeepAlives.
                MissedKeepAlives++;
                SendPacket(new KeepAlive(DateTime.Now.Millisecond));
            
                // TODO: (jake) Allow this to be configured in config.json?
                if (MissedKeepAlives > 5)
                {
                    // TODO: Add player kicking
                    _tcpClient.Close();
                    _tcpClient.Dispose();
                }
            }
        }
        
        #endregion

        #region Player

        /// <summary>
        /// Creates a new Player and assigns it to the Client.
        /// </summary>
        public void CreatePlayer()
        {
            // Checks if a player has already been created.
            if (Player != null)
                throw new Exception("Player is already created.");

            // Switches over the packet state to Play.
            State = State.Play;
            
            // Assigns a new player
            Player = new Player(this, _levelService.GetDefaultLevel());
            try
            {
                Player.Initialize();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            _ = KeepAlive();
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