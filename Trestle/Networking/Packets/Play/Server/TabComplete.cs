using System;
using Trestle.Attributes;
using Trestle.Enums.Packets.Server;

namespace Trestle.Networking.Packets.Play.Server
{
    [ServerBound(PlayPacket.TabComplete)]
    public class TabComplete : Packet
    {
        [Field]
        public string Text { get; set; }
        
        [Field]
        public bool AssumeCommand { get; set; }
        
        [Field]
        public bool HasPosition { get; set; }

        public override void HandlePacket()
        {
            Client.SendPacket(new Client.TabComplete(Text));
        }
    }
}