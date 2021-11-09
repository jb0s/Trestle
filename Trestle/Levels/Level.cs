using Trestle.Levels.Dimensions;
using Trestle.Levels.Enums;
using Trestle.Levels.Generators;

namespace Trestle.Levels
{
    public class Level
    {
        public Dimension Dimension { get; }
        
        public string Name { get; }

        public Generator Generator { get; }
        
        public Level(Dimension dimension, string name, Generator generator)
        {
            Dimension = dimension;
            Name = name;
            Generator = generator;
        }
    }
}