using Trestle.Blocks;

namespace Trestle.Biomes
{
    public interface IBiome
    {
        public int Id { get; }
        public byte MinecraftBiomeId { get; }
        public float Temperature { get; }
        public Block TopBlock { get; }
        public Block Filling { get; }
    }
}