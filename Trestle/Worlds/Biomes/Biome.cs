using Trestle.Blocks;
using Trestle.Enums;
using Trestle.Worlds.Decorators;
using Trestle.Worlds.Structures;

namespace Trestle.Worlds.Biomes
{
    public class Biome : IBiome
    {
        public virtual double BaseHeight => 52.0;

        public virtual int Id => 0;

        public virtual byte MinecraftBiomeId => 0;

        public virtual float Temperature => 0.0f;

        public virtual Decorator[] Decorators => new Decorator[0];

        public virtual Structure[] TreeStructures => new Structure[0];
        
        public virtual Material TopBlock => Material.Grass;

        public virtual Material Filling => Material.Dirt;
    }
}