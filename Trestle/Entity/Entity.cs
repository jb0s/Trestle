using System;
using Trestle.Utils;
using Trestle.Networking.Packets.Play.Client;

namespace Trestle.Entity
{
    /// <summary>
    /// Entities encompass all dynamic, moving objects throughout the Minecraft world.
    /// </summary>
    public class Entity
    {
        /// <summary>
        /// The unique identifier of the entity.
        /// </summary>
        public int EntityId { get; internal set; } = -1;

        /// <summary>
        /// The type of entity this entity identifies as.
        /// TODO: Maybe convert this to an enum?
        /// </summary>
        public int EntityTypeId { get; internal set; } = -1;

        /// <summary>
        /// The world that the entity is in.
        /// </summary>
        public World.World World;

        /// <summary>
        /// Metadata of an Entity.
        /// </summary>
        public Metadata Metadata { get; set; }
        
        /// <summary>
        /// Has the entity spawned in?
        /// </summary>
        public bool IsSpawned;

        /// <summary>
        /// The location of the entity.
        /// </summary>
        public Location Location;
        
        public Entity(int entityTypeId, World.World world)
        {
            World = world;
            Location = new Location(0, 0, 0);
            EntityId = Globals.GetEntityId();
            EntityTypeId = entityTypeId;

            Metadata = new Metadata(this);
        }
        
        /// <summary>
        /// Called every tick as long as the chunk the entity is in is loaded.
        /// </summary>
        public virtual void OnTick()
        {
        }
        
        /// <summary>
        /// Despawns the entity.
        /// </summary>
        public virtual void DespawnEntity()
        {
            foreach (var player in World.Players.Values)
            {
                player.Client.SendPacket(new DespawnEntities(new []{ EntityId }));
            }
            
            World.RemoveEntity(this);
        }
        
        /// <summary>
        /// Spawns the entity.
        /// </summary>
        public virtual void SpawnEntity()
        {
            World.AddEntity(this);
            IsSpawned = true;
        }

        
        /// <summary>
        /// Spawns the entity for a select few players.
        /// </summary>
        /// <param name="players"></param>
        /// <exception cref="NotImplementedException"></exception>
        public virtual void SpawnForPlayers(Player[] players)
        {
            throw new NotImplementedException();
        }
    }
}