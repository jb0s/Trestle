using System;
using Trestle.Networking.Attributes;
using Trestle.Networking.Enums;
using Trestle.Networking.Enums.Server;

namespace Trestle.Networking.Packets.Handshaking.Server
{
    [ServerBound(HandshakingPacket.Handshake)]
    public class Handshake : Packet
    {
        [Field]
        [VarInt]
        public int ProtocolVersion { get; set; }
        
        [Field]
        public string ServerAddress { get; set; }
        
        [Field]
        public ushort ServerPort { get; set; }
        
        [Field(typeof(int))]
        [VarInt]
        public State NextState { get; set; }
        
        public override void Handle()
        {
            Client.State = NextState;
        }
    }
}