using System;
using Trestle.Blocks;
using Trestle.Enums;
using Trestle.Worlds.Biomes;
using Trestle.Worlds.StandardWorld;

namespace Trestle.Worlds.Decorators
{
    public class WaterDecorator : Decorator
    {
        public override void Decorate(ChunkColumn chunk, IBiome biome)
        {
            for (var x = 0; x < 16; x++)
            {
                for (var z = 0; z < 16; z++)
                {
                    //Check for temperature.
                    for (var y = 0; y < StandardWorldGenerator.WaterLevel; y++)
                    {
                        // Lake generation
                        if (y < StandardWorldGenerator.WaterLevel)
                        {
                            if (chunk.GetBlock(x, y, z) == Material.Dirt || chunk.GetBlock(x, y, z) == Material.Grass) //Grass or Dirt?
                            {
                                // Clay
                                if (Globals.Random.Next(1, 40) == 1 && y < StandardWorldGenerator.WaterLevel - 5)
                                    chunk.SetBlock(x, y, z, new Block(Material.Clay));
                                
                                // Dirt as ocean floor
                                else if(y < StandardWorldGenerator.WaterLevel - 3)
                                {
                                    chunk.SetBlock(x, y, z, new Block(Material.Dirt));
                                    chunk.BiomeId[x * 16 + z] = 16;
                                }
                            }
                            if (chunk.GetBlock(x, y + 1, z) == 0)
                            {
                                if (y < StandardWorldGenerator.WaterLevel - 3)
                                {
                                    chunk.SetBlock(x, y + 1, z, new Block(Material.Water)); //Water
                                    chunk.BiomeId[x*16 + z] = 0; //Ocean
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}