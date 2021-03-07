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
        [VarInt]
        public int PrimaryBitMask { get; set; }
        
        [Field]
        public NbtCompound Heightmaps { get; set; }
        
        [Field]
        [VarInt]
        public int BiomesLength { get; set; }
        
        [Field]
        public byte[] Biomes { get; set; }
        
        [Field]
        [VarInt]
        public int Size { get; set; }
        
        [Field]
        public byte[] Data { get; set; }
        
        [Field]
        [VarInt]
        public int BlockEntitiesLength { get; set; }
        
        public bool Unloader = false;

        public ChunkData(ChunkColumn chunk)
        {
            chunk.RecalcHeight();
			
            byte[] sectionData;
            int sectionBitmask = 0;
            using (MemoryStream ms = new())
            {
                var mc = new MinecraftStream(ms.GetBuffer());
				
                for (int i = 0; i < chunk.Sections.Length; i++)
                {
                    ChunkSection section = chunk.Sections[i];

                    if (section.IsEmpty)
                        continue;

                    sectionBitmask |= 1 << i;

                    section.WriteTo(mc, true);
                }

                sectionData = mc.ExportWriter;
            }
            
            ChunkX = chunk.Location.X;
            ChunkZ = chunk.Location.Z;

            PrimaryBitMask = sectionBitmask;
            
            Heightmaps = new NbtCompound("")
            {
                new NbtLongArray("MOTION_BLOCKING", chunk.Heightmap),
                new NbtLongArray("WORLD_SURFACE", chunk.Heightmap)
            };

            BiomesLength = chunk.Biomes.Length;
            
            var stream = new MinecraftStream(new byte[] {});
            for(int i = 0; i < chunk.Biomes.Length; i++)
            {
                stream.WriteVarInt(chunk.Biomes[i]);
            }

            Biomes = stream.ExportWriter;
            Data = sectionData;
            BlockEntitiesLength = 0;
        }
    }
}