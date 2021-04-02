using System;
using Trestle.Blocks;
using Trestle.Worlds.Biomes;
using Trestle.Worlds.TestWorld;

namespace Trestle.Worlds.Decorators
{
    public class GrassDecorator : Decorator
    {
        public override void Decorate(ChunkColumn chunk, IBiome biome, int x, int z)
        {
            for (var y = StandardWorldGenerator.WaterLevel; y < 256; y++)
            {
                if (Globals.Random.Next(0, 10) == 5)
                {
                    if (chunk.GetBlock(x, y, z) == biome.TopBlock)
                        chunk.SetBlock(x, y + 1, z, new Block(31) {Metadata = 1});
                }
                else if (Globals.Random.Next(0, 5000) < 30)
                {
                    if (chunk.GetBlock(x, y, z) == biome.TopBlock)
                    {
                        // TODO: Fix this shit it's awful fuck
                        chunk.SetBlock(x, y + 1, z, new Block(175) {Metadata = 2});
                        chunk.SetBlock(x, y + 2, z, new Block(175) {Metadata = 10});
                    }
                }
            }
        }
    }
}