using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using Trestle.Networking.Attributes;
using Trestle.Networking.Enums;
using Trestle.Utils;

namespace Trestle.Networking.Services
{
    public interface IPacketService
    {
        Packet ParseUncompressedPacket(Client client, NetworkStream stream);
        
        Packet ParseCompressedPacket(NetworkStream stream);
    }
    
    public class PacketService : IPacketService
    {
        // Packets
        private readonly Dictionary<byte, Type> _handshakingPackets = new();
        private readonly Dictionary<byte, Type> _statusPackets = new();
        private readonly Dictionary<byte, Type> _loginPackets = new();
        private readonly Dictionary<byte, Type> _playPackets = new();
        
        public PacketService()
        {
            RegisterPackets();
        }
        
        /// <summary>
        /// 
        /// </summary>
        public Packet ParseUncompressedPacket(Client client, NetworkStream stream)
        {
            using var netty = new NettyStream(stream);
            var packetId = (byte)netty.ReadVarInt();
            
            var type = client.State switch
            {
                State.Handshaking => _handshakingPackets[packetId],
                State.Status => _statusPackets[packetId],
                State.Login => _loginPackets[packetId],
                State.Play => _playPackets[packetId],
                _ => throw new ArgumentOutOfRangeException(nameof(packetId)),
            };
            
            var packet = (Packet)Activator.CreateInstance(type);
            if (packet == null)
                throw new Exception($"Unable to create instance of packet handler {type}");
            
            packet.Client = client;
            packet.Deserialize(netty);
            return packet;
        }

        /// <summary>
        /// 
        /// </summary>
        public Packet ParseCompressedPacket(NetworkStream stream)
        {
            throw new System.NotImplementedException();
        }
        
        /// <summary>
        /// Gets all Packet handlers
        /// </summary>
        private void RegisterPackets()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                var attribute = type.GetCustomAttribute<ServerBoundAttribute>();
                if (attribute == null) 
                    continue;
                
                if (attribute.State == State.Handshaking)
                    _handshakingPackets.Add(attribute.Id, type);
                else if (attribute.State == State.Status)
                    _statusPackets.Add(attribute.Id, type);
                else if (attribute.State == State.Login)
                    _loginPackets.Add(attribute.Id, type);
                else if (attribute.State == State.Play)
                    _playPackets.Add(attribute.Id, type);
            }
        }
    }
}