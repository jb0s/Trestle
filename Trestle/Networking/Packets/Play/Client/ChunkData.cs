using System;
using System.IO;
using fNbt;
using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Utils;
using Trestle.Worlds;

namespace Trestle.Networking.Packets.Play
{
    [ClientBound(PlayPacket.ChunkData)]
    public class ChunkData : Packet
    {
        [Field]
        public int ChunkX { get; set; }
        
        [Field]
        public int ChunkZ { get; set; }

        [Field] 
        public bool IsFullChunk { get; set; } = true;
        
        [Field]
        public ushort PrimaryBitMask { get; set; }
        
        [Field]
        public byte[] Data { get; set; }
        
        public bool Unloader = false;

        public ChunkData(ChunkColumn chunk)
        {
            byte[] sectionData;
            using (MinecraftStream mc = new())
            {
                mc.Write(chunk.GetBytes(false));
                sectionData = mc.Data;
            }
            
            ChunkX = chunk.X;
            ChunkZ = chunk.Z;

            PrimaryBitMask = 0xffff;

            Data = sectionData;
        }
    }
}