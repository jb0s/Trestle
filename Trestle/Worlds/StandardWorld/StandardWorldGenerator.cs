using System;
using Trestle.Enums;
using Trestle.Utils;
using Trestle.Worlds.Biomes;
using Trestle.Worlds.Decorators;
using System.Collections.Generic;
using Trestle.Worlds.Biomes.Desert;
using Trestle.Worlds.Biomes.Forest;
using Trestle.Worlds.Biomes.Plains;

namespace Trestle.Worlds.StandardWorld
{
    public class StandardWorldGenerator : IWorldGenerator
    {
        public const double OVERHANG_OFFSET = 15.0;
        public const double BOTTOM_OFFSET = 0.0;
        public const double OVERHANG_MAGNITUDE = 16.0;
        public const double BOTTOMS_MAGNITUDE = 15.0;
        public const double OVERHANG_SCALE = 128.0;
        public const double GROUND_SCALE = 562.0;
        public const double THRESHOLD = 30;
        public const double BOTTOMS_FREQUENCY = 0.5;
        public const double BOTTOMS_AMPLITUDE = 0.5;
        public const double OVERHANG_FREQUENCY = 0.5;
        public const double OVERHANG_AMPLITUDE = 0.5;
        public const int FILLING_DEEPNESS = 5;
        public const bool ENABLE_OVERHANG = true;
        public static readonly int WaterLevel = 35;

        public Dictionary<Tuple<int, int>, ChunkColumn> ChunkCache = new();
        private BiomeManager _biomeManager;

        public StandardWorldGenerator()
        {
            _biomeManager = new BiomeManager(Config.Seed.GetHashCode());
            _biomeManager.AddBiomeType(new PlainsBiome());
            _biomeManager.AddBiomeType(new ForestBiome());
            _biomeManager.AddBiomeType(new DesertBiome());
        }
        
        public void Initialize()
        {
        }

        public ChunkColumn GenerateChunk(ChunkLocation location)
        {
            ChunkColumn chunk;
            
            if (ChunkCache.TryGetValue(new Tuple<int, int>(location.X, location.Z), out chunk)) 
                return chunk;
            else
                chunk = new ChunkColumn() { X = location.X, Z = location.Z };
            
            PopulateChunk(chunk);

            if (!ChunkCache.ContainsKey(new Tuple<int, int>(location.X, location.Z)))
                ChunkCache.Add(new Tuple<int, int>(location.X, location.Z), chunk);
            
            return chunk;
        }

        public void PopulateChunk(ChunkColumn chunk)
        {
            IBiome biome = null;
            var bottom = new OctaveGenerator(Config.Seed.GetHashCode(), 8);
            var overhang = new OctaveGenerator(Config.Seed.GetHashCode(), 8);

            bottom.SetScale(1 / GROUND_SCALE);
            overhang.SetScale(1 / OVERHANG_SCALE);

            for(int x = 0; x < 16; x++)
            {
                for(int z = 0; z < 16; z++)
                {
                    float ox = x + chunk.X * 16;
                    float oz = z + chunk.Z * 16;

                    biome = _biomeManager.GetBiome((int) ox, (int) oz);
                    chunk.BiomeId[x*16 + z] = biome.MinecraftBiomeId;
                    
                    chunk.Biome = biome;
                    
                    int bottomHeight = (int)(bottom.Noise(ox, oz, BOTTOMS_FREQUENCY, BOTTOMS_AMPLITUDE) * BOTTOMS_MAGNITUDE + BOTTOM_OFFSET + chunk.Biome.BaseHeight);
                    int maxHeight = (int)(overhang.Noise(ox, oz, OVERHANG_FREQUENCY, OVERHANG_AMPLITUDE) * OVERHANG_MAGNITUDE + bottomHeight + OVERHANG_OFFSET);

                    maxHeight = Math.Max(1, maxHeight);

                    for(int y = 0; y < maxHeight && y < 256; y++)
                    {
                        if (y == 0)
                        {
                            chunk.SetBlock(x, y, z, Material.Bedrock);
                            continue;
                        }

                        if (y > bottomHeight)
                        {
                            if (ENABLE_OVERHANG)
                            {
                                var density = overhang.Noise(ox, y, oz, OVERHANG_FREQUENCY, OVERHANG_AMPLITUDE);
                                if (density > THRESHOLD)
                                {
                                    chunk.SetBlock(x, y, z, Material.Stone);
                                }
                            }
                            
                            continue;
                        }
   
                        chunk.SetBlock(x, y, z, Material.Stone);
                    }

                    for(int y = 0; y < 256; y++)
                    {
                        if (chunk.GetBlock(x, y + 1, z) == Material.Air && chunk.GetBlock(x, y, z) == Material.Stone)
                        {
                            chunk.SetBlock(x, y, z, biome.TopBlock);

                            for(int i = 1; i < FILLING_DEEPNESS + 1; i++)
                            {
                                chunk.SetBlock(x, y - i, z, biome.Filling);
                            }
                        }
                    }

                    foreach (var decorator in chunk.Biome.Decorators)
                        decorator.Decorate(chunk, chunk.Biome, x, z);
                }
            }
            
            new WaterDecorator().Decorate(chunk, biome);
        }
    }
}