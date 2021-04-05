using System;
using System.Collections.Concurrent;
using System.Linq;
using Trestle.Entity;
using Trestle.Enums;
using Trestle.Utils;
using Trestle.World.Generation;

namespace Trestle.World
{
    public class WorldManager
    {
        public ConcurrentDictionary<WorldType, World> Worlds { get; } = new();

        /// <summary>
        /// Create a new world.
        /// </summary>
        /// <param name="type">The type of world to create.</param>
        /// <returns></returns>
        public World CreateWorld(WorldType type)
        {
            if (!Worlds.ContainsKey(type))
            {
                var world = new World(type, new FlatWorldGenerator());
                world.Initialize();

                Worlds.TryAdd(type, world);
            }
            
            return Worlds[type];
        }

        /// <summary>
        /// Broadcast a chat message to each world.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="chattype"></param>
        public void BroadcastChat(MessageComponent message, ChatMessageType chattype)
        {
            foreach (var world in Worlds.Values)
                world.BroadcastChat(message, chattype);
        }
    }
}