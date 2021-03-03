using Trestle.Worlds;
using Trestle.Items;

namespace Trestle.Worlds.Decorators
{
    public class LavaDecorator : Decorator
    {
        /// <summary>
        ///     The lava level
        /// </summary>
        public int LavaLevel = 13;

        public override void Decorate(Chunk chunk, Biome biome)
        {
            for (var x = 0; x < 16; x++)
            {
                for (var z = 0; z < 16; z++)
                {
                    for (var y = 0; y < LavaLevel; y++)
                    {
                        //Lake generation
                        if (y < LavaLevel)
                        {
                            if (chunk.GetBlock(x, y + 1, z) == 0)
                            {
                                chunk.SetBlock(x, y + 1, z, new Block(ItemFactory.GetItemById(10))); //Lava
                            }
                        }
                    }
                }
            }
        }
    }
}