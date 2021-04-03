using System;
using Trestle.Attributes;
using Trestle.Enums;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.Client_SpawnObject)]
    public class SpawnObject : Packet
    {
        [Field] 
        [VarInt]
        public int EntityId { get; set; }
        
        [Field]
        public byte Type { get; set; }
        
        [Field]
        public int X { get; set; }
        
        [Field]
        public int Y { get; set; }
        
        [Field]
        public int Z { get; set; }
        
        [Field]
        public byte Pitch { get; set; }
        
        [Field]
        public byte Yaw { get; set; }
        
        [Field]
        public int Info { get; set; }
        
        [Field]
        public short SpeedX { get; set; }
        
        [Field]
        public short SpeedY { get; set; }
        
        [Field]
        public short SpeedZ { get; set; }

        public SpawnObject(byte type, int x, int y, int z, byte pitch, byte yaw, int info)
        {
            Console.WriteLine("Spawning object at " + x + " " + y + " " + z);
            EntityId = Globals.Random.Next(0, 999999999);
            Type = type;
            X = x * 32;
            Y = y * 32;
            Z = z * 32;
            Pitch = pitch;
            Yaw = (byte)((Yaw / 360) * 256);
            Info = info;
        }
    }
}