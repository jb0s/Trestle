using Trestle.Levels.Generators;

namespace Trestle.Levels
{
    public class Level
    {
        public string Name { get; }

        public Generator Generator { get; }
        
        public Level(string name, Generator generator)
        {
            Name = name;
            Generator = generator;
        }
    }
}