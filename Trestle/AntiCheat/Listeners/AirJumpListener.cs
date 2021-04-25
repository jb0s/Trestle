using System;
using Trestle.Enums;
using Trestle.Utils;
using Trestle.Entity;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Trestle.AntiCheat.Listeners
{
    public class AirJumpListener : ICheatListener
    {
        private Dictionary<int, double> _entityYVelocities = new();
        
        public async Task Listen(Player player)
        {
            if (player.GameMode == GameMode.Creative)
                return;
            
            if(!player.IsGrounded && !_entityYVelocities.ContainsKey(player.EntityId))
                _entityYVelocities.Add(player.EntityId, player.Velocity.Y);

            if (player.IsGrounded && _entityYVelocities.ContainsKey(player.EntityId))
                _entityYVelocities.Remove(player.EntityId);
            
            if(!player.IsGrounded && _entityYVelocities.ContainsKey(player.EntityId))
            {
                if (player.Velocity.Y > _entityYVelocities[player.EntityId])
                {
                    OnTriggered(player);
                    return;
                }
                
                _entityYVelocities[player.EntityId] = player.Velocity.Y;
            }
        }

        public async Task OnTriggered(Player player)
        {
            player.Teleport(new Vector3(player.Location.X, 4, player.Location.Z));
            Logger.Warn($"{player.Username} seems to be hacking. Detected AirJump");
        }
    }
}