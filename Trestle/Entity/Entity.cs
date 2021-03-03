using System;
using System.Timers;
using Trestle.Utils;
using Trestle.Worlds;

namespace Trestle.Entity
{
    public class Entity
    {
        public readonly Timer TickTimer = new();

        public int EntityTypeId { get; private set; }
        public int EntityId { get; set; }
        public World World { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public Location Location { get; set; }
        public Vector3 Velocity { get; set; }
        //public HealthManager HealthManager { get; private set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public double Length { get; set; }
        public double Drag { get; set; }
        public double Gravity { get; set; }
        public int Data { get; set; }
        public int Age { get; private set; }
        
        public Entity(int entityTypeId, World world)
        {
            Height = 1;
            Width = 1;
            Length = 1;
            Drag = 0;
            Gravity = 0;
            
            EntityId = EntityManager.GetEntityId();
            World = world;
            EntityTypeId = entityTypeId;
            Location = new Location(0, 0, 0);
            //HealthManager = new HealthManager(this);
        }
        
        public virtual void OnTick()
        {
            Age++;
        }
        
        public virtual void DespawnEntity()
        {
            World.RemoveEntity(this);
        }
        
        public virtual void SpawnEntity()
        {
        }
        
        public byte GetDirection()
        {
            return DirectionByRotationFlat(Location.Yaw);
        }
        
        public Hitbox GetHitbox()
        {
            var pos = Location;
            var halfWidth = Width/2;

            return new Hitbox(new Vector3(pos.X - halfWidth, pos.Y, pos.Z - halfWidth),
                new Vector3(pos.X + halfWidth, pos.Y + Height, pos.Z + halfWidth));
        }
        
        public static byte DirectionByRotationFlat(float yaw)
        {
            var direction = (byte) ((int) Math.Floor((yaw*4F)/360F + 0.5D) & 0x03);
            switch (direction)
            {
                case 0:
                    return 1; // West
                case 1:
                    return 2; // North
                case 2:
                    return 3; // East
                case 3:
                    return 0; // South 
            }
            return 0;
        }
    }
}