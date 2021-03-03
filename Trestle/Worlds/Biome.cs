using Trestle.Worlds;
using Trestle.Worlds.Decorators;
using Trestle.Worlds.Structures;

namespace Trestle.Worlds
{
    public class Biome : IBiome
    {
        public virtual double BaseHeight
        {
            get { return 52.0; }
        }

        public virtual int Id
        {
            get { return 0; }
        }

        public virtual byte MinecraftBiomeId
        {
            get { return 0; }
        }

        public virtual int MaxTrees
        {
            get { return 10; }
        }

        public virtual int MinTrees
        {
            get { return 0; }
        }

        public virtual Structure[] TreeStructures
        {
            get { return new Structure[] {}; }
        }

        public virtual Decorator[] Decorators
        {
            get { return new Decorator[] {}; }
        }

        public virtual float Temperature
        {
            get { return 0.0f; }
        }

        public virtual Block TopBlock
        {
            get { return null; }
        }

        public virtual Block Filling
        {
            get { return new Block(3); }
        }
    }
}