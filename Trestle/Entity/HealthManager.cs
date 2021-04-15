using Trestle.Enums;
using Trestle.Networking.Packets.Play.Client;

namespace Trestle.Entity
{
    public class HealthManager
    {
        public Entity Entity { get; private set; }
        
        public int Health;
        public bool IsDead => Health <= 0;

        public HealthManager(Entity entity)
        {
            Entity = entity;
            Health = Entity.MaxHealth;
        }

        public void Pain(int damage)
        {
            Health -= damage;

            if (Entity is Player)
            {
                var player = (Player)Entity;
                player.Client.SendPacket(new UpdateHealth(player));
                player.PlayerAnimation(AnimationType.TakeDamage);
            }
        }
    }
}