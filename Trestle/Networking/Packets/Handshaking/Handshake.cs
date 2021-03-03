using System;
using Trestle.Attributes;
using Trestle.Enums;

namespace Trestle.Networking.Packets.Handshaking
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
        
        [Field]
        [VarInt]
        public NextState NextState { get; set; }
        
        public override void HandlePacket()
        {
            Client.Protocol = ProtocolVersion;

            if (NextState == NextState.Status) Client.State = ClientState.Status;
            else if (NextState == NextState.Login) Client.State = ClientState.Login;
        }
    }
}