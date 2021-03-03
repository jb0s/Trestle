using Trestle.Worlds;
using Trestle.Items;
using Trestle.Worlds.Standard;

namespace Trestle.Worlds.Decorators
{
    public class OreDecorator : Decorator
    {
        private Chunk _chunke;

        public override void Decorate(Chunk chunk, Biome biome, int x, int z)
        {
            _chunke = chunk;
            for (var y = 1; y < 128; y++)
            {
                if (chunk.GetBlock(x, y, z) == 1)
                {
                    if (y < 128)
                    {
                        GenerateCoal(x, y, z);
                    }

                    if (y < 64)
                    {
                        GenerateIron(x, y, z);
                    }

                    if (y < 29)
                    {
                        GenerateGold(x, y, z);
                    }

                    if (y < 23)
                    {
                        GenerateLapis(x, y, z);
                    }

                    if (y < 16)
                    {
                        if (y > 12)
                        {
                            if (StandardWorldGenerator.GetRandomNumber(0, 3) == 2)
                            {
                                GenerateDiamond(x, y, z);
                            }
                        }
                        else
                        {
                            GenerateDiamond(x, y, z);
                        }
                    }
                }
            }
        }

        public void GenerateCoal(int x, int y, int z)
        {
            if (StandardWorldGenerator.GetRandomNumber(0, 35) == 1)
            {
                _chunke.SetBlock(x, y, z, new Block(ItemFactory.GetItemById(16)));
            }
        }

        public void GenerateIron(int x, int y, int z)
        {
            if (StandardWorldGenerator.GetRandomNumber(0, 65) == 1)
            {
                _chunke.SetBlock(x, y, z, new Block(ItemFactory.GetItemById(15)));
            }
        }

        public void GenerateGold(int x, int y, int z)
        {
            if (StandardWorldGenerator.GetRandomNumber(0, 80) == 1)
            {
                _chunke.SetBlock(x, y, z, new Block(ItemFactory.GetItemById(14)));
            }
        }

        public void GenerateDiamond(int x, int y, int z)
        {
            if (StandardWorldGenerator.GetRandomNumber(0, 130) == 1)
            {
                _chunke.SetBlock(x, y, z, new Block(ItemFactory.GetItemById(56)));
            }
        }

        public void GenerateLapis(int x, int y, int z)
        {
            if (StandardWorldGenerator.GetRandomNumber(0, 80) == 1)
            {
                _chunke.SetBlock(x, y, z, new Block(ItemFactory.GetItemById(21)));
            }
        }
    }
}