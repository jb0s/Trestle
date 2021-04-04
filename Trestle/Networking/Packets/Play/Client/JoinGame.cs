using System;
using System.Linq;
using fNbt;
using Trestle.Attributes;
using Trestle.Entity;
using Trestle.Enums;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.Client_JoinGame)]
    public class JoinGame : Packet
    {
        [Field] 
        public int EntityId { get; set; }

        [Field] 
        public sbyte GameMode { get; set; } = 0x00;

        [Field]
        public sbyte Dimension { get; set; } = 0;

        [Field]
        public byte Difficulty { get; set; } = 3;
        
        [Field]
        public byte MaxPlayers { get; set; }

        [Field]
        public string LevelType { get; set; } = "flat";
        
        [Field] 
        public bool ShowReducedDebugInfo { get; set; } = false;

        public JoinGame(Player player)
        {
            EntityId = player.EntityId;
            GameMode = (sbyte)player.GameMode;
            
            MaxPlayers = Config.MaxPlayers;
        }
    }
}