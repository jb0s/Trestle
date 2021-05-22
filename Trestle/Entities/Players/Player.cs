using System;
using Trestle.Entities.Enums;
using Trestle.Networking;
using Trestle.Networking.Packets.Play.Client;
using Trestle.Worlds;

namespace Trestle.Entities.Players
{
    public class Player : Entity
    {
        /// <summary>
        /// Client that the Player is associated with.
        /// </summary>
        public Client Client { get; }

        public Player(Client client, World world) : base(EntityType.Player, world)
        {
            Client = client;
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