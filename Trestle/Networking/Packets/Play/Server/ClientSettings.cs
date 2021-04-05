using System;
using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Enums.Packets.Server;
using Trestle.Networking.Packets.Play.Client;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Play.Server
{
    [ServerBound(PlayPacket.ClientSettings)]
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
            
                /*
            using var stream = new MinecraftStream();
            stream.WriteByte((0 << 5 | 10 & 0x1F) & 0xFF);
            stream.WriteByte(DisplayedSkinParts);
            stream.WriteByte(0x7F);
            Client.SendPacket(new EntityMetadata(Client.Player.EntityId, stream.Data));
            */
            // TODO: Fix this shit it's broken
            player.SendChunksForLocation();
        }
    }
}