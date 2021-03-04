using System.Collections.Concurrent;
using System.Linq;
using Trestle.Entity;
using Trestle.Worlds.Generation;

namespace Trestle.Worlds
{
    public class WorldManager
    {
        private ConcurrentDictionary<string, World> Worlds { get; } = new();

        public World MainWorld 
            => Worlds.FirstOrDefault().Value;
        
        public virtual World GetLevel(Player player, string name)
            => Worlds.GetOrAdd(name, CreateWorld);

        public World CreateWorld(string s)
        {
            World world = new(s, new FlatWorldGenerator());
            world.Initialize();

            Worlds.TryAdd(s, world);
            
            return world;
        }

        public virtual void RemoveWorld(World world)
        {
            World oldWorld;
            
            if (Worlds.TryRemove(world.Name, out oldWorld))
                world.Dispose();
        }

        public World[] GetWorlds()
            => Worlds.Values.ToArray();
    }
}