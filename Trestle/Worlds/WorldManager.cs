using System.Collections.Generic;
using System.Linq;

namespace Trestle.Worlds
{
    public class WorldManager
    {
        public World MainWorld { get; private set; }
        public Dictionary<string, World> Worlds { get; private set; }
        
        public WorldManager(World mainWorld)
        {
            MainWorld = mainWorld;
            Worlds = new Dictionary<string, World>();
        }
        
        public void AddLevel(string name, World world)
        {
            Worlds.Add(name, world);
        }

        private World GetLevel(string name)
        {
            var worldCursor = (from world in Worlds where world.Key == name select world.Value).FirstOrDefault();
            if (worldCursor != null) return worldCursor;
            return MainWorld;
        }
    }
}