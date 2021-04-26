using Trestle.Attributes;
using Trestle.Enums.Packets.Client;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.Particle)]
    public class Particle : Packet
    {
        [Field]
        public int ParticleId { get; set; }
        
        [Field]
        public bool LongDistance { get; set; }
        
        [Field]
        public float X { get; set; }
        
        [Field]
        public float Y { get; set; }
        
        [Field]
        public float Z { get; set; }

        [Field]
        public float OffsetX { get; set; } = 0;

        [Field]
        public float OffsetY { get; set; } = 0;

        [Field]
        public float OffsetZ { get; set; } = 0;

        [Field]
        public float ParticleData { get; set; } = 0;

        [Field]
        public int ParticleCount { get; set; } = 100;
        
        public int[] Data { get; set; }

        public Particle(int particleId, float x, float y, float z)
        {
            ParticleId = particleId;

            X = x;
            Y = y;
            Z = z;

            OffsetX = Globals.Random.Next(1, 2);
            OffsetY = Globals.Random.Next(1, 3);
            OffsetZ = Globals.Random.Next(1, 2);
            
            Data = new int[2];
        }
    }
}