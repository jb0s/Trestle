using Trestle.Enums;
using Trestle.Worlds.Standard;

namespace Trestle.Worlds.Decorators
{
    public class FlowerDecorator : Decorator
    {
        public override void Decorate(Chunk chunk, Biome biome)
        {
        }

        public override void Decorate(Chunk chunk, Biome biome, int x, int z)
        {
            for (var y = StandardWorldGenerator.WaterLevel; y < 256; y++)
            {
                if (StandardWorldGenerator.GetRandomNumber(0, 5000) < 50)
                {
                    if (chunk.GetBlock(x, y, z) == biome.TopBlock.Id)
                    {
                        chunk.SetBlock(x, y + 1, z, new Block(Material.RedFlower) {Metadata = (byte)(StandardWorldGenerator.GetRandomNumber(0, 7) + 1)});
                    }
                }
            }
        }
    }
}