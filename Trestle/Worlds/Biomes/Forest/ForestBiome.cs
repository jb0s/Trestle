using Trestle.Blocks;
using Trestle.Enums;
using Trestle.Worlds.Decorators;
using Trestle.Worlds.Structures;

namespace Trestle.Worlds.Biomes.Forest
{
    public class ForestBiome : Biome
    {
        public override int Id => 1;
        public override byte MinecraftBiomeId => 4;
        public override Decorator[] Decorators => new Decorator[] { new GrassDecorator(), new ForestDecorator() };
        public override Structure[] TreeStructures => new Structure[] { new OakTree() };
        public override float Temperature => 0.7f;
        public override Material TopBlock => Material.Grass;
    }
}