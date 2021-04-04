using System;
using Trestle.Blocks;
using Trestle.Enums;
using Trestle.Worlds.Biomes;
using Trestle.Worlds.StandardWorld;

namespace Trestle.Worlds.Decorators
{
    public class CactusDecorator : Decorator
    {
        public override void Decorate(ChunkColumn chunk, IBiome biome, int x, int z)
        {
            for (var y = StandardWorldGenerator.WaterLevel; y < 256; y++)
            {
                if (Globals.Random.Next(0, 1000) == 0)
                {
                    if (chunk.GetBlock(x, y, z) == biome.TopBlock && chunk.GetBlock(x, y + 1, z) == Material.Air)
                    {
                        int height = Globals.Random.Next(2, 5);
                        for(int i = 0; i < height; i++)
                            chunk.SetBlock(x, y + 1 + i, z, new Block(Material.Cactus));
                    }
                }
            }
        }
    }
}