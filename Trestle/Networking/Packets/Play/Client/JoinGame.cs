using System;
using System.Linq;
using Trestle.Attributes;
using Trestle.Entity;
using Trestle.Enums;
using Trestle.Enums.Packets.Client;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.JoinGame)]
    public class JoinGame : Packet
    {
        [Field] 
        public int EntityId { get; set; }

        [Field] 
        public byte GameMode { get; set; } = 0x01;

        [Field]
        public int Dimension { get; set; } = 0;

        [Field]
        public byte Difficulty { get; set; } = 3;
        
        [Field]
        public byte MaxPlayers { get; set; }

        // TODO: make dynamic to world type
        [Field]
        public string LevelType { get; set; } = "flat";
        
        [Field] 
        public bool ShowReducedDebugInfo { get; set; } = false;

        public JoinGame(Player player)
        {
            EntityId = player.EntityId;
            GameMode = (byte)player.GameMode;
            
            MaxPlayers = Config.MaxPlayers;
        }
    }
}