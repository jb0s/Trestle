using System.Collections.Concurrent;
using Trestle.Worlds.Enums;

namespace Trestle.Worlds.Services
{
    public interface IWorldService
    {
        
    }
    
    public class WorldService : IWorldService
    {
        private readonly ConcurrentDictionary<string, World> _worlds = new ();
        
        public WorldService()
        {
            
        }
    }
}