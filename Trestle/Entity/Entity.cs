using System;
using System.Diagnostics;
using Trestle.Enums;
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
        /// </summary>
        public EntityType EntityType { get; internal set; } = EntityType.Player;

        /// <summary>
        /// The world that the entity is in.
        /// </summary>
        public Worlds.World World;

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

        /// <summary>
        /// The entity's healthmanager.
        /// </summary>
        public HealthManager HealthManager;

        /// <summary>
        /// The entity's velocity.
        /// </summary>
        public Vector3 Velocity;
        
        /// <summary>
        /// The maximum amount of health this entity can have.
        /// </summary>
        public virtual int MaxHealth => 0;

        public Entity(EntityType entityType, Worlds.World world)
        {
            World = world;
            Location = new Location(0, 0, 0);
            EntityId = Globals.GetEntityId();
            EntityType = entityType;

            Metadata = new Metadata(this);
            HealthManager = new HealthManager(this);
        }

        /// <summary>
        /// Called every tick as long as the chunk the entity is in is loaded.
        /// </summary>
        public virtual void OnTick()
            => HealthManager.OnTick();

        /// <summary>
        /// Despawns the entity.
        /// </summary>
        public virtual void DespawnEntity()
        {
            foreach (var player in World.Players.Values)
            {
                player.Client.SendPacket(new DestroyEntities(new []{ EntityId }));
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

        public void Knockback(Entity entityIn, float strength, double xRatio, double zRatio)
        {
            float f = (float)Math.Sqrt(xRatio * xRatio + zRatio * zRatio);
            double velX = 0;
            double velY = 0;
            double velZ = 0;
            
            velX /= 2.0D;
            velZ /= 2.0D;

            velX -= xRatio / (double)f * (double)strength;
            velZ -= zRatio / (double)f * (double)strength;

            if (Location.OnGround)
            {
                velY /= 2.0D;
                velY += (double)strength;
                
                // why so specific, notch?
                if (velY > 0.4000000059604645D)
                    velY = 0.4000000059604645D;
            }
            
            World.BroadcastPacket(new EntityVelocity(EntityId, new Vector3(velX, velY, -velZ) * 8000));
        }

        /// <summary>
        /// Spawns the entity for a select few players.
        /// </summary>
        public virtual void SpawnForPlayers(Player[] players)
            => throw new Exception("This method is not to be called. It needs to be overridden.");
    }
}