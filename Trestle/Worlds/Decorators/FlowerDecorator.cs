using Trestle.Blocks;
using Trestle.Enums;
using Trestle.Worlds.Biomes;
using Trestle.Worlds.StandardWorld;

namespace Trestle.Worlds.Decorators
{
    public class FlowerDecorator : Decorator
    {
        public override void Decorate(ChunkColumn chunk, IBiome biome, int x, int z)
        {
            for (int y = StandardWorldGenerator.WaterLevel; y < 256; y++)
                if (Globals.Random.Next(0, 5000) < 50)
                    if (chunk.GetBlock(x, y, z) == biome.TopBlock)
                        chunk.SetBlock(x, y + 1, z, new Block(Material.RedFlower) {Metadata = (byte)(Globals.Random.Next(0, 7) + 1)});
        }
    }
}