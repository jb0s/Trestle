using System;
using System.IO;
using System.Linq;
using Trestle.Items;
using Trestle.Utils;
using Trestle.Worlds;
using System.Collections.Generic;
using Trestle.Worlds.BiomeSystem;
using Trestle.Worlds.Decorators;

namespace Trestle.Worlds.Standard
{
    public class StandardWorldGenerator : WorldGenerator
    {
        private const double OVERHANG_OFFSET = 15.0; // Canges the offset from the bottom ground.
        private const double BOTTOM_OFFSET = 0.0; // Changes the offset from y level 0
        private const double OVERHANGS_MAGNITUDE = 16.0;
        private const double BOTTOMS_MAGNITUDE = 15.0;
        private const double OVERHANG_SCALE = 128.0; // Changes the scale of the overhang.
        private const double HEIGHT_LIMIT = 256.0; // Changes the height limit.
        private const double THRESHOLD = 30;
        private const double BOTTOMS_FREQUENCY = 0.5;
        private const double BOTTOMS_AMPLITUDE = 0.5;
        private const double OVERHANG_FREQUENCY = 0.5;
        private const double OVERHANG_AMPLITUDE = 0.5;
        private const bool ENABLE_OVERHANG = true;

        public static int WaterLevel = 50;
        private static readonly object SyncLock = new ();
        private readonly BiomeManager _biomeManager;
        private readonly CaveGenerator _caveGen = new(Config.Seed.GetHashCode());
        private readonly string _folder;
        public Dictionary<Tuple<int, int>, Chunk> ChunkCache = new ();
        public sealed override bool IsCaching { get; set; }
        
        public StandardWorldGenerator(string folder, World myWorld) : base(myWorld)
        {
	        _folder = folder;

	        if (!Directory.Exists(_folder))
		        Directory.CreateDirectory(_folder);
	        
	        IsCaching = true;
	        _biomeManager = new BiomeManager(Config.Seed.GetHashCode());
	        _biomeManager.AddBiomeType(new PlainsBiome());
	        _biomeManager.AddBiomeType(new ForestBiome());
        }
        
        public override Vector3 GetSpawnPoint()
        {
	        return new(0, 82, 0);
        }
        
        public override Chunk LoadChunk(int x, int z)
        {
            var u = Globals.Decompress(File.ReadAllBytes(_folder + "/" + x + "." + z + ".cfile"));
            var reader = new DataBuffer(u);

            var blockLength = reader.ReadInt();
            var block = reader.ReadUShortLocal(blockLength);

            var metalength = reader.ReadInt();
            var blockmeta = reader.ReadUShortLocal(metalength);

            var skyLength = reader.ReadInt();
            var skylight = reader.Read(skyLength);

            var lightLength = reader.ReadInt();
            var blocklight = reader.Read(lightLength);

            var biomeIdLength = reader.ReadInt();
            var biomeId = reader.Read(biomeIdLength);

            List<ForcedBlock> forcedStructuresForChunk = new List<ForcedBlock>();
            if (MyWorld.BlocksToForce.ContainsKey($"{x}{z}"))
	            forcedStructuresForChunk = MyWorld.BlocksToForce[$"{x}{z}"];
            
            var cc = new Chunk(MyWorld, x, z, forcedStructuresForChunk)
            {
                Blocks = block,
                Metadata = blockmeta,
               // Blocklight = {Data = blocklight},
              //  Skylight = {Data = skylight},
                BiomeId = biomeId,
                X = x,
                Z = z
            };
            Console.WriteLine("We should have loaded " + x + ", " + z);
            return cc;
        }
        
        public override void SaveChunks(string folder)
        {
            lock (ChunkCache)
            {
                foreach (var i in ChunkCache.Values.ToArray())
                {
                    if (!i.IsDirty)
                    {
                        SaveChunk(i);
                    }
                }
            }
        }
        
        private bool SaveChunk(Chunk chunk)
        {
            File.WriteAllBytes(_folder + "/" + chunk.X + "." + chunk.Z + ".cfile", Globals.Compress(chunk.Export()));
            return true;
        }
        
        public override Chunk GenerateChunk(Vector2 chunkCoordinates)
        {
            Chunk c;
            if (ChunkCache.TryGetValue(new Tuple<int, int>(chunkCoordinates.X, chunkCoordinates.Z), out c)) return c;

            if (File.Exists((_folder + "/" + chunkCoordinates.X + "." + chunkCoordinates.Z + ".cfile")))
            {
                var cd = LoadChunk(chunkCoordinates.X, chunkCoordinates.Z);
                lock (ChunkCache)
                {
                    if (!ChunkCache.ContainsKey(new Tuple<int, int>(cd.X, cd.Z)))
                        ChunkCache.Add(new Tuple<int, int>(cd.X, cd.Z), cd);
                }
                return cd;
            }

            List<ForcedBlock> forcedStructuresForChunk = new List<ForcedBlock>();

            if (MyWorld.BlocksToForce.ContainsKey($"{chunkCoordinates.X}{chunkCoordinates.Z}"))
            {
	            forcedStructuresForChunk = MyWorld.BlocksToForce[$"{chunkCoordinates.X}{chunkCoordinates.Z}"];
	            MyWorld.BlocksToForce.Remove($"{chunkCoordinates.X}{chunkCoordinates.Z}");
            }

			var chunk = new Chunk(MyWorld, chunkCoordinates.X, chunkCoordinates.Z, forcedStructuresForChunk);
            PopulateChunk(chunk);

            if (!ChunkCache.ContainsKey(new Tuple<int, int>(chunkCoordinates.X, chunkCoordinates.Z)))
                ChunkCache.Add(new Tuple<int, int>(chunkCoordinates.X, chunkCoordinates.Z), chunk);

            return chunk;
        }
        
        private void PopulateChunk(Chunk chunk)
		{
			var bottom = new SimplexOctaveGenerator(Config.Seed.GetHashCode(), 8);
			var overhang = new SimplexOctaveGenerator(Config.Seed.GetHashCode(), 8);
			overhang.SetScale(1 / OVERHANG_SCALE);
			bottom.SetScale(1 / HEIGHT_LIMIT);

			for (var x = 0; x < 16; x++)
			{
				for (var z = 0; z < 16; z++)
				{
					float ox = x + chunk.X*16;
					float oz = z + chunk.Z*16;

					var cBiome = _biomeManager.GetBiome((int) ox, (int) oz);
					chunk.BiomeId[x*16 + z] = cBiome.MinecraftBiomeId;

					// Set the chunk's biome to the biome.
					chunk.Biome = cBiome;

					var bottomHeight =
						(int)
							((bottom.Noise(ox, oz, BOTTOMS_FREQUENCY, BOTTOMS_AMPLITUDE) * BOTTOMS_MAGNITUDE) + BOTTOM_OFFSET + cBiome.BaseHeight);

					var maxHeight = (int)((overhang.Noise(ox, oz, OVERHANG_FREQUENCY, OVERHANG_AMPLITUDE) * OVERHANGS_MAGNITUDE) + bottomHeight + OVERHANG_OFFSET);
					maxHeight = Math.Max(1, maxHeight);

					for (var y = 0; y < maxHeight && y < 256; y++)
					{
						if (y == 0)
						{
							chunk.SetBlock(x, y, z, new Block(ItemFactory.GetItemById(7)));
							continue;
						}

						if (y > bottomHeight)
						{
							//part where we do the overhangs
							if (ENABLE_OVERHANG)
							{
								var density = overhang.Noise(ox, y, oz, OVERHANG_FREQUENCY, OVERHANG_AMPLITUDE);
								if (density > THRESHOLD) chunk.SetBlock(x, y, z, new Block(ItemFactory.GetItemById(1)));
							}
						}
						else
						{
							chunk.SetBlock(x, y, z, new Block(ItemFactory.GetItemById(1)));
						}
					}

					//Turn the blocks ontop into the correct material
					for (var y = 0; y < 256; y++)
					{
						if (chunk.GetBlock(x, y + 1, z) == 0 && chunk.GetBlock(x, y, z) == 1)
						{
							chunk.SetBlock(x, y, z, cBiome.TopBlock);

							chunk.SetBlock(x, y - 1, z, cBiome.Filling);
							chunk.SetBlock(x, y - 2, z, cBiome.Filling);
						}
					}

					foreach (var decorator in cBiome.Decorators)
					{
						decorator.Decorate(chunk, cBiome, x, z);
					}
					new OreDecorator().Decorate(chunk, cBiome, x, z);
					new BedrockDecorator().Decorate(chunk, cBiome, x, z);
				}
			}

			new WaterDecorator().Decorate(chunk, new PlainsBiome());
			_caveGen.GenerateCave(chunk);
			new LavaDecorator().Decorate(chunk, new PlainsBiome());
		}
        
        public static int GetRandomNumber(int min, int max)
        {
	        lock (SyncLock)
	        {
		        return Globals.Random.Next(min, max);
	        }
        }
    }
}