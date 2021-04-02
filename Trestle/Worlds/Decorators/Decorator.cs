using Trestle.Worlds.Biomes;

namespace Trestle.Worlds.Decorators
{
    public class Decorator
    {
        public virtual void Decorate(ChunkColumn chunk, IBiome biome)
        {
        }
        
        public virtual void Decorate(ChunkColumn chunk, IBiome biome, int x, int z)
        {
        }
    }
}