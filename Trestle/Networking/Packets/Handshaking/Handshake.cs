using System;
using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Networking.Packets.Login.Client;
using Trestle.Utils;

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
            
            if (NextState == NextState.Status)
                Client.State = ClientState.Status;
            else if (NextState == NextState.Login)
            {
                if (Client.Protocol < Globals.ProtocolVersion)
                    Client.SendPacket(new Disconnect($"Outdated client! I'm on {Globals.OfficialProtocolName.Replace("Minecraft ", "")}"));

                if (Client.Protocol > Globals.ProtocolVersion)
                    Client.SendPacket(new Disconnect($"Client too new! I'm still on {Globals.OfficialProtocolName.Replace("Minecraft ", "")}"));
                
                Client.State = ClientState.Login;
            }
        }
    }
}