using Trestle.Enums;

namespace Trestle.Worlds.Standard
{
    internal class StandardWorld : World
    {
        public StandardWorld(string worldName)
        {
            Difficulty = 0;
            Name = worldName;
            WorldType = WorldType.Default;
            Generator = new StandardWorldGenerator(worldName, this);
            DefaultGameMode = GameMode.Creative;
        }
    }
}