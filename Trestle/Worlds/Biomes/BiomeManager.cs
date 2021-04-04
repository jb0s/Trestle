using System;
using System.Collections.Generic;
using Trestle.Worlds.StandardWorld;

namespace Trestle.Worlds.Biomes
{
    public class BiomeManager
    {
        private static readonly List<Biome> Biomes = new ();
        private readonly OctaveGenerator _octaveGeneratorb;
        private readonly double _biomeHeigth = 14.5;
        private readonly double _biomeWidth = 12.3;

        public BiomeManager(int seed)
        {
            _octaveGeneratorb = new OctaveGenerator(seed, 1);
            _octaveGeneratorb.SetScale(1/200.0);
        }

        public Biome GetBiome(int x, int z)
        {
            x = (int) Math.Floor((decimal) (x/_biomeWidth));
            z = (int) Math.Floor((decimal) (z/_biomeHeigth));

            var b = (int) Math.Abs(_octaveGeneratorb.Noise(x, z, 1, 5) * (Biomes.Count + 2));
            if (b >= Biomes.Count) b = Biomes.Count - 1;
            
            return Biomes[b];
        }

        public void AddBiomeType(Biome biome)
        {
            Biomes.Add(biome);
        }

        public static Biome GetBiomeById(int id, bool minecraftId = true)
        {
            foreach (var b in Biomes)
            {
                if (minecraftId)
                {
                    if (b.MinecraftBiomeId == id)
                    {
                        return b;
                    }
                }
                else
                {
                    if (b.Id == id)
                    {
                        return b;
                    }
                }
            }
            return null;
        }
    }
}