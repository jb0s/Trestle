using System;
using Trestle.Blocks;
using Trestle.Enums;
using Trestle.Worlds.Biomes;
using Trestle.Worlds.TestWorld;

namespace Trestle.Worlds.Decorators
{
    public class DeadBushDecorator : Decorator
    {
        public override void Decorate(ChunkColumn chunk, IBiome biome, int x, int z)
        {
            for (var y = StandardWorldGenerator.WaterLevel; y < 256; y++)
            {
                if (Globals.Random.Next(0, 100) < 2)
                {
                    if (chunk.GetBlock(x, y, z) == biome.TopBlock && chunk.GetBlock(x, y + 1, z) == Material.Air)
                        chunk.SetBlock(x, y + 1, z, new Block(Material.DeadBush));
                }
            }
        }
    }
}