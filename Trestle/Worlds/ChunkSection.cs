using System;
using Trestle.Enums;
using Trestle.Utils;

namespace Trestle.Worlds
{
    public class ChunkSection
    {
        public const int HEIGHT = 16;
        public const int WIDTH_DEPTH = 16;
        public const int TOTAL_BLOCKS = HEIGHT * WIDTH_DEPTH * WIDTH_DEPTH;

        public CompactedDataArray Blocks;
        public NibbleArray Skylight;
        public NibbleArray Blocklight;

        public bool IsEmpty => _airBlocks == TOTAL_BLOCKS;
        
        private int _airBlocks = TOTAL_BLOCKS;

        public ChunkSection()
        {
            Skylight = new NibbleArray(TOTAL_BLOCKS);
            Blocklight = new NibbleArray(TOTAL_BLOCKS);

            Blocks = new CompactedDataArray(13, TOTAL_BLOCKS);
            
            for(int i = 0; i < TOTAL_BLOCKS; i++)
            {
                Blocks[i] = 0;
                Skylight[i] = 0xff;
            }
        }

        public void RecalculateAir()
        {
            _airBlocks = 0;
            
            for(int i = 0; i < TOTAL_BLOCKS; i++)
                if ((Blocks[i] >> 4).GetMaterial() == Material.Air)
                    _airBlocks++;
        }
        
        private static int GetIndex(int x, int y, int z)
        {
            if (x < 0 || z < 0 || y < 0 || x >= WIDTH_DEPTH || z >= WIDTH_DEPTH || y >= HEIGHT)
                throw new IndexOutOfRangeException($"Invalid coordinates! ({x}, {y}, {z})");

            return (y << 8) | (z << 4) | x;
        }
        
        public Material GetBlockMaterial(int x, int y, int z)
            => (Blocks[GetIndex(x, y, z)] >> 4).GetMaterial();
        
        public byte GetBlockData(int x, int y, int z)
            => (byte) (Blocks[GetIndex(x, y, z)] & 15);
        
        public void SetBlockId(int x, int y, int z, short id)
        {
            var index = GetIndex(x, y, z);
            var data = Blocks[index];

            int type = data >> 4;
            int metadata = data & 15;

            if (type == 0 && id > 0)
                _airBlocks--;
            
            else if (type > 0 && id == 0)
                _airBlocks++;

            Blocks[index] = (id << 4 | (metadata & 15));
        }
        
        public void SetBlockData(int x, int y, int z, byte meta)
        {
            var index = GetIndex(x, y, z);
            var data = Blocks[index];
            int type = data >> 4;

            Blocks[index] = (type << 4 | (meta & 15));
        }
        
        public void WriteTo(MinecraftStream stream)
        {
            var types = Blocks.Backing;

            stream.WriteShort((short)(TOTAL_BLOCKS - _airBlocks)); // Block count
            stream.WriteByte(13); // Bits per block
            
            stream.WriteVarInt(Blocks.Capacity); // Palette length
            for(int i = 0; i < Blocks.Capacity; i++)
                stream.WriteVarInt(Blocks[i]);
            
            stream.WriteVarInt(types.Length); // Data Array Length
            for (int i = 0; i < types.Length; i++) // Data Array
                stream.WriteLong(types[i]);
        }
    }
}