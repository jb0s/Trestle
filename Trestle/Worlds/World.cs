using System.Collections.Concurrent;
using Trestle.Entities;
using Trestle.Entities.Players;
using Trestle.Utils;
using Trestle.Worlds.Enums;

namespace Trestle.Worlds
{
    public class World
    {
        /// <summary>
        /// Dictionary of all Players in the world.
        /// </summary>
        public ConcurrentDictionary<int, Player> Players { get; } = new();
        
        /// <summary>
        /// Dictionary of all Entities in the world.
        /// </summary>
        public ConcurrentDictionary<int, Entity> Entities { get; } = new();

        /// <summary>
        /// Default <see cref="GameMode"/> that players are put in when spawned.
        /// </summary>
        public GameMode DefaultGameMode { get; set; } = GameMode.Survival;

    }
}