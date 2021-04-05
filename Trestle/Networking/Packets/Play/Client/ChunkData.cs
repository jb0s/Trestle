using System;
using System.IO;
using fNbt;
using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Enums.Packets.Client;
using Trestle.Utils;
using Trestle.Worlds;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.ChunkData)]
    public class ChunkData : Packet
    {
        [Field]
        public int ChunkX { get; set; }
        
        [Field]
        public int ChunkZ { get; set; }

        [Field] 
        public bool IsGroundUpContinuous { get; set; } = true;
        
        [Field]
        [VarInt]
        public int PrimaryBitMask { get; set; }
        
        [Field]
        [VarInt]
        public int Size { get; set; }
        
        [Field]
        public byte[] Data { get; set; }
        
        [Field]
        [VarInt]
        public int NumberOfBlockEntities { get; set; }

        public bool Unloader = false;

        public ChunkData(ChunkColumn chunk)
        {
            byte[] sectionData;
            using (MinecraftStream mc = new())
            {
                mc.Write(chunk.GetBytes(Unloader));
                sectionData = mc.Data;
            }
            
            ChunkX = chunk.X;
            ChunkZ = chunk.Z;

            PrimaryBitMask = 0xffff;

            Size = sectionData.Length;
            // TODO: fix
            Data = sectionData;
            
            NumberOfBlockEntities = 0;
        }
    }
}