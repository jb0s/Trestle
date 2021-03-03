using Trestle.Worlds;
using Trestle.Worlds.Decorators;
using Trestle.Worlds.Structures;

namespace Trestle.Worlds
{
    public interface IBiome
    {
        int Id { get; }
        byte MinecraftBiomeId { get; }
        Structure[] TreeStructures { get; }
        Decorator[] Decorators { get; }
        float Temperature { get; }
        Block TopBlock { get; }
        Block Filling { get; }
    }
}