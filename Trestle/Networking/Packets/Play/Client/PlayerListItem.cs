using System;
using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Enums.Packets.Client;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.PlayerListItem)]
    public class PlayerListItem : Packet
    {
        [Field]
        [VarInt]
        public int Action { get; set; }
        
        [Field]
        [VarInt]
        public int NumberOfPlayers { get; set; }

        [Field]
        public byte[] Players { get; set; }
        
        // Add Player
        public PlayerListItem(params UserProfile[] players)
        {
            Action = 0;
            NumberOfPlayers = players.Length;

            var stream = new MinecraftStream();
            foreach (var player in players)
            {
                stream.WriteUuid(Guid.Parse(player.Id));
                stream.WriteString(player.Name); 
                stream.WriteVarInt(player.Properties.Length);
                foreach (var property in player.Properties)
                {
                    stream.WriteString(property.Name);
                    stream.WriteString(property.Value);
                    stream.WriteBool(property.Signature != null);
                    if (property.Signature != null)
                        stream.WriteString(property.Signature);
                }
                
                stream.WriteVarInt(0);
                stream.WriteVarInt(0);
                stream.WriteBool(false);
            }

            Players = stream.Data;
        }
    }
}