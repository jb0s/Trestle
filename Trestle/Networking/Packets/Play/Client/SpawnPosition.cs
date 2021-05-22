using System;
using Trestle.Entities.Players;
using Trestle.Networking.Attributes;
using Trestle.Networking.Enums.Client;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.SpawnPosition)]
    public class SpawnPosition : Packet
    {
        [Field]
        public long Location { get; set; }
        
        public SpawnPosition(Player player)
        {
            var data = new Location(0, 0, 0);
            Location = (((long) data.X & 0x3FFFFFF) << 38) | (((long) data.Z & 0x3FFFFFF) << 12) |
                       ((long) data.Y & 0xFFF);
        }
    }
}