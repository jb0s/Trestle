using System;
using System.Timers;
using Trestle.Networking;
using Trestle.Networking.Packets.Play.Client;
using Trestle.Utils;
using Trestle.Worlds;

namespace Trestle.Entity
{
    public class Entity
    {
        public int EntityId { get; internal set; } = -1;

        public World World;
        public bool IsSpawned;

        public DateTime LastUpdated;
        public Location Location;
        
        public Entity(int entityTypeId, World world)
        {
            World = world;
            Location = new Location(0, 0, 0);
            EntityId = Globals.GetEntityId();
        }
        
        public virtual void OnTick()
        {
        }
        
        public virtual void DespawnEntity()
        {
            foreach (var player in World.Players.Values)
            {
                player.Client.SendPacket(new DespawnEntities(new []{ EntityId }));
            }
            
            World.RemoveEntity(this);
        }
        
        public virtual void SpawnEntity()
        {
            World.AddEntity(this);
            IsSpawned = true;
        }

        public virtual void SpawnForPlayers(Player[] players)
        {
        }

        public virtual void DespawnForPlayers(Player[] players)
        {
        }
    }
}