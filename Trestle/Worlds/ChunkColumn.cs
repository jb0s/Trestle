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
        private readonly ChunkSection[] _sections;
        
        public bool IsDirty { get; private set; } = false;

        public long[] Heightmap = new long[WIDTH_DEPTH * WIDTH_DEPTH];
        private byte[] _biomes = new byte[WIDTH_DEPTH * WIDTH_DEPTH];
        private byte[] _cache = null;

        public ChunkColumn(ChunkLocation location)
        {
            location = location;
            _sections = new ChunkSection[16];

            for (int i = 0; i < _sections.Length; i++)
            {
                _sections[i] = new ChunkSection();
            }

            for (int i = 0; i < _biomes.Length; i++)
            {
                _biomes[i] = 1; //Plains
            }

            for (int i = 0; i < Heightmap.Length; i++)
            {
                Heightmap[i] = 0;
            }
        }
        
        private ChunkSection GetChunkSection(int y)
			=> _sections[y >> 4];

        public Material GetBlockMaterial(int x, int y, int z)
			=> GetChunkSection(y).GetBlockMaterial(x, y - 16 * (y >> 4), z);

		public void SetBlockId(int x, int y, int z, short id)
		{
			GetChunkSection(y).SetBlockId(x, y - 16*(y >> 4), z, id);

			_cache = null;
			IsDirty = true;
		}

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
			_biomes[(z << 4) + (x)] = biome;

			_cache = null;
			IsDirty = true;
		}

		public byte GetBiome(int x, int z)
			=> _biomes[(z << 4) + (x)];

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

		public byte[] ToArray()
		{
			using (MemoryStream ms = new())
			{
				var m = new MinecraftStream(ms.GetBuffer());
				
				WriteTo(m);
				
				return ms.ToArray();
			}
		}

		public void WriteTo(MinecraftStream stream)
		{
			if (_cache != null)
			{
				stream.Write(_cache);
				return;
			}

			RecalcHeight();
			
			byte[] sectionData;
			int sectionBitmask = 0;
			using (MemoryStream ms = new())
			{
				var mc = new MinecraftStream(ms.GetBuffer());
				
				for (int i = 0; i < _sections.Length; i++)
				{
					ChunkSection section = _sections[i];
					
					if (section.IsEmpty) 
						continue;

					sectionBitmask |= 1 << i;

					section.WriteTo(mc, true);
				}
				
				sectionData = ms.ToArray();
			}

			using (MemoryStream ms = new())
			{
				var mc = new MinecraftStream(ms.GetBuffer());
				mc.WriteInt(Location.X); // Chunk X
				mc.WriteInt(Location.Z); // Chunk Z

				mc.WriteBool(true); // Full chunk

				mc.WriteVarInt(sectionBitmask); // Primary Bit Mask

				var streamm = new MemoryStream();
				var thing = new NbtCompound("")
				{
					new NbtList("MOTION_BLOCKING", Heightmap.Select(x => new NbtLong(x)))
				};
				new NbtFile(thing).SaveToStream(streamm, NbtCompression.None);
				
				mc.Write(streamm.ToArray());
				
				mc.WriteInt(1); // Biomes length
				mc.Write(_biomes); // Biomes
				
				mc.WriteVarInt(sectionData.Length + 256); // Size
				mc.Write(sectionData, 0, sectionData.Length); // Data

				mc.WriteVarInt(0); // Number of block entities

				_cache = ms.ToArray();
			}

			stream.Write(_cache);
		}
    }
}