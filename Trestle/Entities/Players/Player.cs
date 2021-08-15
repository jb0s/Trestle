using System;
using System.Collections.Generic;
using Trestle.Entities.Enums;
using Trestle.Levels;
using Trestle.Networking;
using Trestle.Networking.Packets.Play.Client;
using Trestle.Utils;

namespace Trestle.Entities.Players
{
    public class Player : Entity
    {
        /// <summary>
        /// Client that the Player is associated with.
        /// </summary>
        public Client Client { get; }

        /// <summary>
        /// The chunks this entity has loaded.
        /// </summary>
        public Dictionary<Tuple<int, int>, byte[]> ChunksUsed = new();
        
        public Player(Client client, Level level) : base(EntityType.Player, level)
        {
            Client = client;
            Location = new Location(0, 4, 0);
        }

        public void Initialize()
        {
            Client.SendPacket(new JoinGame(this));
            
            // Spawns the player
            Client.SendPacket(new SpawnPosition(this));
            Client.SendPacket(new PlayerPositionAndLook(this));
        }
    }
}