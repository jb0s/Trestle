using Trestle.Worlds.Standard;

namespace Trestle.Worlds.Decorators
{
    public class ForestDectorator : Decorator
    {
        private void GenerateTree(Chunk chunk, int x, int y, int z, Biome biome)
        {
            if (biome.TreeStructures.Length != 0)
            {
                var random = StandardWorldGenerator.GetRandomNumber(0, biome.TreeStructures.Length);
                biome.TreeStructures[random].Create(chunk, x, y, z);
            }
        }

        public override void Decorate(Chunk chunk, Biome biome, int x, int z)
        {
            for(int y = StandardWorldGenerator.WaterLevel; y < 256; y++)
            {
                if (StandardWorldGenerator.GetRandomNumber(0, 5000) < 20)
                {
                    if(chunk.GetBlock(x, y + 1, z) == biome.TopBlock.Id)
                        GenerateTree(chunk, x, y + 1, z, biome);
                }
            }
        }
    }
}