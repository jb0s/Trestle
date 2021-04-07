using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Networking.Packets.Login.Client;
using Trestle.Utils;

namespace Trestle.Networking
{
    public class Listener
    {
        /// <summary>
        /// Are we listening for incoming connections?
        /// </summary>
        private bool _isListening = false;

        /// <summary>
        /// The socket that listens to incoming connections.
        /// </summary>
        private TcpListener _listener = new(IPAddress.Any, Config.Port);

        /// <summary>
        /// A list of clients that are connected to the server.
        /// </summary>
        public List<Client> Clients { get; private set; } = new();

        private readonly Dictionary<byte, Type> _handshakingPackets = new();
        private readonly Dictionary<byte, Type> _statusPackets = new();
        private readonly Dictionary<byte, Type> _loginPackets = new();
        private readonly Dictionary<byte, Type> _playPackets = new();

        #region Intialization

        /// <summary>
        /// Initializes the listener and starts listening.
        /// </summary>
        internal void Start()
        {
            LoadHandlers();
            
            _listener.Start();
            _isListening = true;
            
            Logger.Info($"Accepting connections on port {Config.Port}");
            
            while (_isListening)
            {
                var client = _listener.AcceptTcpClient();
                new Task(() => HandleConnection(client)).Start();
            }
            
            // We're not listening for connections anymore, shut down the server.
            if(!TrestleServer.Stopped)
                TrestleServer.Shutdown();
        }

        /// <summary>
        /// Stops the listener and disconnects any established connections.
        /// </summary>
        internal void Stop()
        {
            _listener.Stop();
            _isListening = false;

            // Disconnect any established connections.
            foreach (var client in Clients)
            {
                if(client.Player != null)
                    client.Player.Kick(new MessageComponent("Server closed"));
                else
                    client.SendPacket(new Disconnect(new MessageComponent("Server closed")));
            }
        }

        /// <summary>
        /// In charge of defining the packet handlers.
        /// </summary>
        internal void LoadHandlers()
        {
            foreach(var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                var attribute = (ServerBoundAttribute)Attribute.GetCustomAttribute(type, typeof(ServerBoundAttribute));
                if (attribute == null) 
                    continue;
                
                if (attribute.State == ClientState.Handshaking)
                    _handshakingPackets.Add(attribute.Id, type);
                else if (attribute.State == ClientState.Status)
                    _statusPackets.Add(attribute.Id, type);
                else if (attribute.State == ClientState.Login)
                    _loginPackets.Add(attribute.Id, type);
                else if (attribute.State == ClientState.Play)
                    _playPackets.Add(attribute.Id, type);
            }
        }
        
        #endregion

        #region Packet handling

        /// <summary>
        /// Handles an incoming connection.
        /// </summary>
        internal void HandleConnection(TcpClient tcpClient)
        {
            var stream = tcpClient.GetStream();
            var client = new Client(tcpClient);
            Clients.Add(client);
            
            while (tcpClient.Connected)
            {
                try
                {
                    while (!stream.DataAvailable)
                    {
                        if (client.Kicked)
                            break;
                        
                        Thread.Sleep(5);
                    }
                    
                    if (client.Kicked)
                        break;

                    // TODO: add support for compressed packets & some other logic
                    HandleUncompressedPacket(client, stream);
                }
                catch (Exception ex)
                {
                }
            }

            // Client lost connection, remove.
            TrestleServer.UnregisterPlayer(client);
            Clients.Remove(client);
        }

        /// <summary>
        /// Handles an uncompressed packet.
        /// </summary>
        /// <param name="client">The connected client that sent this packet.</param>
        /// <param name="stream">The incoming packet.</param>
        internal void HandleUncompressedPacket(Client client, NetworkStream stream)
        {
            int length = ReadVarInt(stream);
            byte[] buffer = new byte[length];
            int receivedData = stream.Read(buffer, 0, buffer.Length);

            if (receivedData > 0)
            {
                var dbuffer = new MinecraftStream(client);
                if (client.Decrypter != null)
                {
                    byte[] data = new byte[4096];
                    client.Decrypter.TransformBlock(buffer, 0, buffer.Length, data, 0);
                    dbuffer.BufferedData = data;
                }

                dbuffer.BufferedData = buffer;
                dbuffer.Size = length;
                
                byte packetId = (byte)dbuffer.ReadVarInt();
                
                HandlePacket(client, dbuffer, packetId);
                
                dbuffer.Dispose();
            }
        }

        /// <summary>
        /// Handles a compressed packet.
        /// </summary>
        /// <param name="client">The connected client that sent this packet.</param>
        /// <param name="buffer">The incoming packet.</param>
        /// <param name="packetId">The identifier of the incoming packet.</param>
        internal void HandlePacket(Client client, MinecraftStream buffer, byte packetId)
        {
            var type = client.State switch
            {
                ClientState.Handshaking => _handshakingPackets.GetValue(packetId),
                ClientState.Status => _statusPackets.GetValue(packetId),
                ClientState.Login => _loginPackets.GetValue(packetId),
                ClientState.Play => _playPackets.GetValue(packetId),
            };

            if (type == null)
            {
                Logger.Warn($"Unknown packet '0x{packetId:X2}' for state '{client.State}'");
                return;
            }
            
            try
            {
                var packet = (Packet)Activator.CreateInstance(type);
                if (packet == null)
                    throw new Exception($"Unable to create instance of packet handler {type}");
                
                packet.Client = client;

                packet.DeserializePacket(buffer);
                packet.HandlePacket();
            }
            catch (Exception e)
            {
                if(type.GetCustomAttribute<IgnoreExceptionsAttribute>() == null)
                    client.Player?.Kick(new MessageComponent($"{ChatColor.Red}An exception occurred while handling packet.\n\n{ChatColor.Reset}{e.Message}\n{ChatColor.DarkGray}{e.StackTrace}"));
            }
        }

        #endregion

        #region Utilities

        internal int ReadVarInt(NetworkStream stream)
        {
            int value = 0;
            int size = 0;
            int b;

            while (((b = stream.ReadByte()) & 0x80) == 0x80)
            {
                value |= (b & 0x7F) << (size++ * 7);
                
                if (size > 5)
                    throw new IOException("VarInt too long!");
            }

            return value | ((b & 0x7F) << (size * 7));
        }

        #endregion
    }
}