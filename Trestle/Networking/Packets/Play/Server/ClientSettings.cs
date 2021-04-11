using System;
using Trestle.Attributes;
using Trestle.Entity;
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
        
        [Field(typeof(byte))]
        public SkinParts DisplayedSkinParts { get; set; }

        public override void HandlePacket()
        {
            var player = Client.Player;

            player.Locale = Locale;
            player.ViewDistance = ViewDistance;
            player.ChatColours = ChatColors;
            player.SkinParts = DisplayedSkinParts;
            
            Client.Player.World.BroadcastPacket(new EntityMetadata(Client.Player));
            
            player.SendChunksForLocation();
        }
    }
}