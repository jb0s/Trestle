namespace Trestle.Worlds.Decorators
{
    public class Decorator
    {
        public virtual void Decorate(Chunk chunk, Biome biome)
        {
        }

        public virtual void Decorate(Chunk chunk, Biome biome, int x, int z)
        {
            Decorate(chunk, biome);
        }
    }
}