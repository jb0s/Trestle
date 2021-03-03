﻿using Trestle.Attributes;
using Trestle.Enums;

namespace Trestle.Networking.Packets.Play
{
    [ClientBound(PlayPacket.SpawnPosition)]
    public class SpawnPosition : Packet
    {
        [Field]
        public long Location { get; set; }

        public SpawnPosition()
        {
            var spawnPoint = Globals.WorldManager.MainWorld.GetSpawnPoint();
            Location = (((long) spawnPoint.X & 0x3FFFFFF) << 38) | (((long) spawnPoint.Y & 0xFFF) << 26) | ((long) spawnPoint.Z & 0x3FFFFFF);
        }
    }
}