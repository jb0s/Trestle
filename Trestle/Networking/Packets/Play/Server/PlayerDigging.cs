using System;
using Trestle.Attributes;
using Trestle.Entity;
using Trestle.Enums;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Play.Server
{
    [ServerBound(PlayPacket.Server_PlayerDigging)]
    public class PlayerDigging : Packet
    {
        [Field]
        public byte Status { get; set; }
        
        [Field]
        public Vector3 Location { get; set; }
        
        [Field]
        public byte Face { get; set; }

        public override void HandlePacket()
        {
            if (Status == 2)
            {
                var block = Client.Player.World.GetBlock(Location);
                block.BreakBlock(Client.Player.World);

                // Don't drop itemstacks in creative, adventure or spectator
                if (Client.Player.GameMode == GameMode.Survival)
                {
                    new ItemEntity(Client.Player.World, new ItemStack(new Item(block.Id, 0), 1))
                    {
                        Location = new Location(Location.X, Location.Y, Location.Z)
                    }.SpawnEntity();
                }
            }
        }
    }
}