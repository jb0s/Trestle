using Trestle.Worlds.Biomes;
using Trestle.Worlds.StandardWorld;

namespace Trestle.Worlds.Decorators
{
    public class ForestDecorator : Decorator
    {
        private void GenerateTree(ChunkColumn chunk, int x, int y, int z, IBiome biome)
        {
            if (biome.TreeStructures.Length != 0)
            {
                var random = Globals.Random.Next(0, biome.TreeStructures.Length);
                biome.TreeStructures[random].Create(chunk, x, y, z);
            }
        }

        public override void Decorate(ChunkColumn chunk, IBiome biome, int x, int z)
        {
            for(int y = StandardWorldGenerator.WaterLevel; y < 256; y++)
            {
                if (Globals.Random.Next(0, 5000) < 20)
                {
                    if(chunk.GetBlock(x, y + 1, z) == biome.TopBlock)
                        GenerateTree(chunk, x, y + 1, z, biome);
                }
            }
        }
    }
}