using Trestle.Blocks;
using Trestle.Enums;
using Trestle.Worlds.Decorators;
using Trestle.Worlds.Structures;

namespace Trestle.Worlds.Biomes
{
    public interface IBiome
    {
        public int Id { get; }
        public byte MinecraftBiomeId { get; }
        public float Temperature { get; }
        public Material TopBlock { get; }
        public Material Filling { get; }
        public double BaseHeight { get; }
        public Decorator[] Decorators { get; }
        public Structure[] TreeStructures { get; }
    }
}