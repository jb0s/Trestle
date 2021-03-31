using Trestle.Blocks;
using Trestle.Enums;

namespace Trestle.Biomes
{
    public class BaseBiome : IBiome
    {
        public virtual double BaseHeight => 52.0;

        public virtual int Id => 0;

        public virtual byte MinecraftBiomeId => 0;

        public virtual int MaxTrees => 10;

        public virtual int MinTrees => 0;
        
        public virtual float Temperature => 0.0f;

        public virtual Block TopBlock => new(Material.Grass);

        public virtual Block Filling => new(Material.Dirt);
    }
}