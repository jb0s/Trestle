﻿using Trestle.Enums;
using Trestle.Worlds;
using Trestle.Worlds.Standard;

namespace Trestle.Worlds.Decorators
{
    public class GrassDecorator : Decorator
    {
        public override void Decorate(Chunk chunk, Biome biome)
        {
        }

        public override void Decorate(Chunk chunk, Biome biome, int x, int z)
        {
            for (var y = StandardWorldGenerator.WaterLevel; y < 256; y++)
            {
                if (StandardWorldGenerator.GetRandomNumber(0, 10) == 5)
                {
                    if (chunk.GetBlock(x, y, z) == biome.TopBlock.Id)
                    {
                        chunk.SetBlock(x, y + 1, z, new Block(31) {Metadata = 1});
                    }
                }
                else if (StandardWorldGenerator.GetRandomNumber(0, 5000) < 30)
                {
                    if (chunk.GetBlock(x, y, z) == biome.TopBlock.Id)
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