using System;
using System.Numerics;
using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Enums.Packets.Client;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.SpawnObject)]
    public class SpawnObject : Packet
    {
        [Field] 
        [VarInt]
        public int EntityId { get; set; }
        
        [Field]
        public Guid Uuid { get; set; } = Guid.NewGuid();
        
        [Field]
        public byte Type { get; set; }
        
        [Field]
        public double X { get; set; }

        [Field]
        public double Y { get; set; }
        
        [Field]
        public double Z { get; set; }
        
        [Field]
        public byte Pitch { get; set; }
        
        [Field]
        public byte Yaw { get; set; }
        
        [Field]
        public int Data { get; set; }
        
        [Field]
        public Velocity Velocity { get; set; }

        public SpawnObject(Entity.Entity entity, Location location, byte type, int data)
        {
            EntityId = entity.EntityId;

            X = location.X;
            Y = location.Y;
            Z = location.Z;
            
            Type = type;
            Data = data;

            Velocity = new Velocity(0, 0, 0);
        }
    }
}