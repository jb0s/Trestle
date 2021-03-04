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
using Trestle.Utils;

namespace Trestle.Networking
{
    public class Listener
    {
        private TcpListener _listener = new(IPAddress.Any, Config.Port);

        private bool _isListening = false;

        private Dictionary<byte, Func<Packet>> _handshakingHandlers = new();
        private Dictionary<byte, Func<Packet>> _statusHandlers = new();
        private Dictionary<byte, Func<Packet>> _loginHandlers = new();
        private Dictionary<byte, Func<Packet>> _playHandlers = new();

        public List<Client> Clients { get; private set; } = new();
        
        public void Start()
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
        }

        private void HandleConnection(TcpClient tcpClient)
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
            Logger.Info(client.Player.Username + " lost connection");
            Clients.Remove(client);
        }

        private void HandleUncompressedPacket(Client client, NetworkStream stream)
        {
            var length = ReadVarInt(stream);
            var buffer = new byte[length];
            var receivedData = stream.Read(buffer, 0, buffer.Length);

            if (receivedData > 0)
            {
                var dbuffer = new DataBuffer(client);
                if (client.Decrypter != null)
                {
                    var data = new byte[4096];
                    client.Decrypter.TransformBlock(buffer, 0, buffer.Length, data, 0);
                    dbuffer.BufferedData = data;
                }

                dbuffer.BufferedData = buffer;
                dbuffer.Size = length;
                
                var packetId = (byte)dbuffer.ReadVarInt();
                
                HandlePacket(client, dbuffer, packetId);
                
                dbuffer.Dispose();
            }
        }

        private void HandlePacket(Client client, DataBuffer buffer, byte packetId)
        {
            var handler = client.State switch
            {
                ClientState.Handshaking => _handshakingHandlers.GetValue(packetId),
                ClientState.Status => _statusHandlers.GetValue(packetId),
                ClientState.Login => _loginHandlers.GetValue(packetId),
                ClientState.Play => _playHandlers.GetValue(packetId),
            };

            if (handler == null)
            {
                Logger.Warn($"Unknown packet '0x{packetId:X2}' for state '{client.State}'");
                return;
            }
            
            var type = client.State switch
            {
                ClientState.Handshaking => typeof(HandshakingPacket),
                ClientState.Status => typeof(StatusPacket),
                ClientState.Login => typeof(LoginPacket),
                ClientState.Play => typeof(PlayPacket)
            };
            Logger.Debug($"Received packet '0x{packetId:X2}' ({Enum.GetName(type, packetId)}) for state '{client.State}'");

            var packet = handler();
            packet.Client = client;
            
            packet.DeserializePacket(buffer);
            packet.HandlePacket();
        }
        
        private void LoadHandlers()
        {
            foreach(Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                var attribute = (ServerBoundAttribute)Attribute.GetCustomAttribute(type, typeof(ServerBoundAttribute));
                if (attribute != null)
                {
                    if (attribute.State == ClientState.Handshaking)
                        _handshakingHandlers.Add(attribute.Id, () => (Packet)Activator.CreateInstance(type));
                    else if (attribute.State == ClientState.Status)
                        _statusHandlers.Add(attribute.Id, () => (Packet)Activator.CreateInstance(type));
                    else if (attribute.State == ClientState.Login)
                        _loginHandlers.Add(attribute.Id, () => (Packet)Activator.CreateInstance(type));
                    else if (attribute.State == ClientState.Play)
                        _playHandlers.Add(attribute.Id, () => (Packet)Activator.CreateInstance(type));
                }
            }
        }
        
        private int ReadVarInt(NetworkStream stream)
        {
            var value = 0;
            var size = 0;
            int b;

            while (((b = stream.ReadByte()) & 0x80) == 0x80)
            {
                value |= (b & 0x7F) << (size++ * 7);
                
                if (size > 5)
                    throw new IOException("VarInt too long!");
            }

            return value | ((b & 0x7F) << (size * 7));
        }
    }
}