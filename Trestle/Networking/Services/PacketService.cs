using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Logging;
using Trestle.Enums;
using Trestle.Levels.Items;
using Trestle.Levels.Items.Services;
using Trestle.Networking.Attributes;
using Trestle.Networking.Enums;
using Trestle.Utils;

namespace Trestle.Networking.Services
{
    public interface IPacketService
    {
        /// <summary>
        /// Parses an uncompressed packet.
        /// </summary>
        Packet ParseUncompressedPacket(Client client, NetworkStream stream);
        
        /// <summary>
        /// Parses a compressed packet.
        /// </summary>
        Packet ParseCompressedPacket(Client client, NetworkStream stream);
    }
    
    public class PacketService : IPacketService
    {
        // Packets
        private readonly Dictionary<byte, Type> _handshakingPackets = new();
        private readonly Dictionary<byte, Type> _statusPackets = new();
        private readonly Dictionary<byte, Type> _loginPackets = new();
        private readonly Dictionary<byte, Type> _playPackets = new();
        
        private ILogger<PacketService> _logger { get; set; }

        public PacketService(ILogger<PacketService> logger, IItemService itemService)
        {
            _logger = logger;

            RegisterPackets();
        }
        
        public Packet ParseUncompressedPacket(Client client, NetworkStream stream)
        {
            using var netty = new NettyStream(stream);
            var packetId = (byte)netty.ReadVarInt();

            _logger.LogDebug($"Attempting to handle packet '0x{packetId:X2}' in state '{client.State}'");
            
            Type type;
            var doesPacketExist = client.State switch
            {
                State.Handshaking => _handshakingPackets.TryGetValue(packetId, out type),
                State.Status =>  _statusPackets.TryGetValue(packetId, out type),
                State.Login =>  _loginPackets.TryGetValue(packetId, out type),
                State.Play =>  _playPackets.TryGetValue(packetId, out type),
                _ => throw new ArgumentOutOfRangeException(nameof(packetId)),
            };
            
            if (!doesPacketExist)
            {
                _logger.LogWarning($"Packet '0x{packetId:X2}' does not have a handler for state '{client.State}'");
                return null;
            }
            
            var packet = (Packet)Activator.CreateInstance(type);
            if (packet == null)
                throw new Exception($"Unable to create instance of packet handler {type}");
            
            packet.Client = client;
            packet.Deserialize(netty);
            return packet;
        }
        
        public Packet ParseCompressedPacket(Client client, NetworkStream stream)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Registers all Packet handlers
        /// </summary>
        private void RegisterPackets()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                var attribute = type.GetCustomAttribute<ServerBoundAttribute>();
                if (attribute == null) 
                    continue;
                
                switch (attribute.State)
                {
                    case State.Handshaking:
                        _handshakingPackets.Add(attribute.Id, type);
                        break;
                    case State.Status:
                        _statusPackets.Add(attribute.Id, type);
                        break;
                    case State.Login:
                        _loginPackets.Add(attribute.Id, type);
                        break;
                    case State.Play:
                        _playPackets.Add(attribute.Id, type);
                        break;
                }
            }
        }
    }
}