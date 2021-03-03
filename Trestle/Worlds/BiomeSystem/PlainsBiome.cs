using Trestle.Enums;
using Trestle.Worlds;
using Trestle.Worlds.Decorators;

namespace Trestle.Worlds.BiomeSystem
{
    internal class PlainsBiome : Biome
    {
        public override int Id => 0;
        public override byte MinecraftBiomeId => 1;
        public override Decorator[] Decorators => new Decorator[] {new GrassDecorator(), new FlowerDecorator()};
        public override float Temperature => 0.8f;
        public override Block TopBlock => new(Material.Grass);
    }
}