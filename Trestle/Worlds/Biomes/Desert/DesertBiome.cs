using Trestle.Enums;
using Trestle.Worlds.Decorators;
using Trestle.Worlds.Structures;

namespace Trestle.Worlds.Biomes.Desert
{
    public class DesertBiome : Biome
    {
        public override int Id => 2;
        public override byte MinecraftBiomeId => 2;
        public override Decorator[] Decorators => new Decorator[] { new CactusDecorator(), new DeadBushDecorator() };
        public override float Temperature => 5f;
        public override Material TopBlock => Material.Sand;
        public override Material Filling => Material.Sandstone;
    }
}