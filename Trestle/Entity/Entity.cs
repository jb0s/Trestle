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
        
        /// <summary>
        /// The location the player was in last tick.
        /// </summary>
        protected Vector3 PreviousLocation;

        /// <summary>
        /// Is the entity grounded?
        /// Calculated server-side to fight NoFall.
        /// </summary>
        public bool IsGrounded => 
            World.GetBlock(Location.ToVector3() - new Vector3(0, 0.5, 0)).Material != Material.Air;

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
        {
            PreviousLocation = Location.ToVector3();
            
            HealthManager.OnTick();
        }

        public virtual void PositionChanged(Vector3 location, float yaw = 0.0f, float pitch = 0.0f, bool onGround = false)
        {
            if (yaw != 0.0f && pitch != 0.0f)
            {
                Location.Yaw = yaw;
                Location.Pitch = pitch;
                Location.HeadYaw = (byte)(yaw * 256 / 360);
            }
            
            Location.X = location.X;
            Location.Y = location.Y; 
            Location.Z = location.Z;
            Location.OnGround = onGround;
            
            Velocity = new Vector3(location.X - PreviousLocation.X, location.Y - PreviousLocation.Y, location.Z - PreviousLocation.Z) * 20;
        }
            
        
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

        /// <summary>
        /// Spawns the entity for a select few players.
        /// </summary>
        public virtual void SpawnForPlayers(Player[] players)
            => throw new Exception("This method is not to be called. It needs to be overridden.");
    }
}