using System;
using Trestle.Attributes;
using Trestle.Entity;
using Trestle.Enums.Packets.Client;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.SpawnPlayer)]
    public class SpawnPlayer : Packet
    {
        [Field]
        [VarInt]
        public int EntityId { get; set; }
        
        [Field]
        public Guid Uuid { get; set; }
        
        [Field]
        public double X { get; set; }
        
        [Field]
        public double Y { get; set; }
        
        [Field]
        public double Z { get; set; }
        
        [Field]
        public byte Yaw { get; set; }
        
        [Field]
        public byte Pitch { get; set; }

        [Field] 
        public byte[] Metadata { get; set; }

        public SpawnPlayer(Player player)
        {
            var metadata = new PlayerMetadata();
            metadata.SkinMask = player.SkinParts;

            Metadata = metadata.ToArray();
        }
    }
}