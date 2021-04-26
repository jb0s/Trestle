using System;
using System.Diagnostics;
using System.Linq;
using Trestle.Enums;
using Trestle.Items;
using Trestle.Networking;
using Trestle.Networking.Packets.Play.Client;
using Trestle.Utils;
using EntityStatus = Trestle.Networking.Packets.Play.Client.EntityStatus;

namespace Trestle.Entity
{
    public class HealthManager
    {
        public Entity Entity { get; private set; }
        
        public int Health;
        public bool IsDead => Health <= 0;
        
        public bool IsInvulnerable => _invulerabilityTimer > 0;
        private float _invulerabilityTimer = 0;

        private Stopwatch _voidDamageStopwatch;
        private bool _isFalling;
        private float _yVelocity;

        public HealthManager(Entity entity)
        {
            Entity = entity;
            Health = Entity.MaxHealth;
            
            _voidDamageStopwatch = new Stopwatch();
        }

        public void OnTick()
        {
            if (IsDead)
                return;

            if(IsInvulnerable)
                _invulerabilityTimer -= 1;

            // Entity starts falling, keep track of its velocity.
            if (!Entity.Location.OnGround && Entity.Velocity.Y < -12)
            {
                // We do this because the Y velocity is set back to 0 before the player registers as being grounded.
                _yVelocity = (float)Entity.Velocity.Y;
                _isFalling = true;
            }

            // Entity lands on ground, calculate & apply fall damage.
            if (Entity.Location.OnGround && _isFalling)
            {
                _isFalling = false;

                int pain = (int)Math.Abs(_yVelocity / 2.5);
                if (pain != 0)
                    Pain(pain, false);

                _yVelocity = 0;
            }
            
            // Void damage
            if (Entity.Location.Y < -20)
            {
                if(!_voidDamageStopwatch.IsRunning)
                    _voidDamageStopwatch.Start();

                if (_voidDamageStopwatch.ElapsedMilliseconds > 500)
                {
                    Pain(4, true);
                    _voidDamageStopwatch.Restart();
                }
            }
            else if (_voidDamageStopwatch.IsRunning)
            {
                _voidDamageStopwatch.Reset();
                _voidDamageStopwatch.Stop();
            }
        }
        
        public bool Pain(int damage, bool bypassInvulnerable = false)
        {
            // Invulnerability needs to be bypassed for void damage.
            if (IsInvulnerable && !bypassInvulnerable)
                return false;

            // Make the player invulnerable for 10 ticks.
            _invulerabilityTimer = 10;
            
            Health -= damage;

            UpdateHealth();
            return true;
        }

        public void Die()
        {
            Health = 0;
            
            UpdateHealth();
        }

        public void UpdateHealth()
        {
            if (Entity is Player)
            {
                var player = (Player)Entity;
                var metadata = (PlayerMetadata)player.Metadata;

                // If health is less than or equal to 0, and a totem could not be used, drop the inventory.
                if (Health <= 0 && !player.AttemptTotem())
                    player.Inventory.DropAll();
                
                metadata.Health = Health;
                player.World.BroadcastPacket(new EntityMetadata(player));
                
                player.PlayerAnimation(AnimationType.TakeDamage);
                player.World.BroadcastPacket(new NamedSoundEffect("entity.player.hurt", SoundCategory.Player, player.Location.ToVector3(), 1f, 1f));
                
                player.Client.SendPacket(new UpdateHealth(player));
            }
        }

        public void Reset()
        {
            Health = Entity.MaxHealth;
            _isFalling = false;
            _yVelocity = 0;
            
            if (Entity is Player)
            {
                var player = (Player)Entity;
                var metadata = (PlayerMetadata)player.Metadata;

                metadata.Health = Health;
                
                // Destroy previous entity and make new one.
                // TODO: Make this better?
                player.World.BroadcastPacket(new DestroyEntities(new int[1] { player.EntityId }));
                player.World.BroadcastPacket(new SpawnPlayer(player));
                
                player.Client.SendPacket(new EntityMetadata(player));
                player.Client.SendPacket(new UpdateHealth(player));
            }
        }
    }
}