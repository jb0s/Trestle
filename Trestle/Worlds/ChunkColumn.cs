using System.IO;
using System.Linq;
using fNbt;
using LibNoise.Modifier;
using Trestle.Enums;
using Trestle.Utils;

namespace Trestle.Worlds
{
    public class ChunkColumn
    {
        public const int HEIGHT = 256;
        public const int WIDTH_DEPTH = 16;

        public readonly ChunkLocation Location;
        public readonly ChunkSection[] Sections;
        public byte[] Biomes = new byte[WIDTH_DEPTH * WIDTH_DEPTH];

        public bool IsDirty { get; private set; } = false;

        public long[] Heightmap = new long[WIDTH_DEPTH * WIDTH_DEPTH];
        private byte[] _cache = null;

        public ChunkColumn(ChunkLocation location)
        {
            location = location;
            Sections = new ChunkSection[16];

            for (int i = 0; i < Sections.Length; i++)
            {
                Sections[i] = new ChunkSection();
            }

            for (int i = 0; i < Biomes.Length; i++)
            {
                Biomes[i] = 1; //Plains
            }

            for (int i = 0; i < Heightmap.Length; i++)
            {
                Heightmap[i] = 0;
            }
        }
        
        private ChunkSection GetChunkSection(int y)
			=> Sections[y >> 4];

        public Material GetBlockMaterial(int x, int y, int z)
			=> GetChunkSection(y).GetBlockMaterial(x, y - 16 * (y >> 4), z);

		public void SetBlockId(int x, int y, int z, short id)
		{
			GetChunkSection(y).SetBlockId(x, y - 16*(y >> 4), z, id);

			_cache = null;
			IsDirty = true;
		}
		
		public void SetBlockId(int x, int y, int z, Material id)
			=> SetBlockId(x, y, z, (short)id);

		public byte GetBlockData(int x, int y, int z)
		{
			return GetChunkSection(y).GetBlockData(x, y - 16 * (y >> 4), z);
		}

		public void SetBlockData(int x, int y, int z, byte meta)
		{
			GetChunkSection(y).SetBlockData(x, y - 16 * (y >> 4), z, meta);

			_cache = null;
			IsDirty = true;
		}

		public void SetBiome(int x, int z, byte biome)
		{
			Biomes[(z << 4) + (x)] = biome;

			_cache = null;
			IsDirty = true;
		}

		public byte GetBiome(int x, int z)
			=> Biomes[(z << 4) + (x)];

		public void RecalcHeight()
		{
			for (int x = 0; x < 16; x++)
				for (int z = 0; z < 16; z++)
					for (byte y = 127; y > 0; y--)
						if (GetBlockMaterial(x, y, z) != Material.Air)
						{
							Heightmap[(x << 4) + z] = (long)(y + 1);
							break;
						}
		}
    }
}