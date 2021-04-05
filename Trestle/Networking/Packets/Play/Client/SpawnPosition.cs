using System;
using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Enums.Packets.Client;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.SpawnPosition)]
    public class SpawnPosition : Packet
    {
        [Field]
        public long Location { get; set; }

        public SpawnPosition()
        {
            var spawnPoint = Client.Player.World.Spawnpoint;
            
            Location = ((((long) spawnPoint.X & 0x3FFFFFF) << 38) | (((long) spawnPoint.Z & 0x3FFFFFF) << 12) |
                        ((long) spawnPoint.Y & 0xFFF));
        }
    }
}