using System;
using Trestle.Attributes;
using Trestle.Enums;

namespace Trestle.Networking.Packets.Play.Server
{
    [ServerBound(PlayPacket.Server_ClientSettings)]
    public class ClientSettings : Packet
    {
        [Field]
        public string Locale { get; set; }
        
        [Field]
        public byte ViewDistance { get; set; }
        
        [Field]
        public byte ChatMode { get; set; }
        
        [Field]
        public bool ChatColors { get; set; }
        
        [Field]
        public byte DisplayedSkinParts { get; set; }

        public override void HandlePacket()
        {
            var player = Client.Player;

            player.Locale = Locale;
            player.ViewDistance = ViewDistance;
            // TODO: ChatMode
            player.ChatColours = ChatColors;
            player.SkinParts = DisplayedSkinParts;
            
            // TODO: Fix this shit it's broken
            player.SendChunksForLocation();
        }
    }
}