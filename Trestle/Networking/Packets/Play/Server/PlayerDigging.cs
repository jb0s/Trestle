using System;
using Trestle.Attributes;
using Trestle.Entity;
using Trestle.Enums;
using Trestle.Enums.Packets.Server;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Play.Server
{
    [ServerBound(PlayPacket.PlayerDigging)]
    public class PlayerDigging : Packet
    {
        [Field(typeof(byte))]
        public PlayerDiggingStatus Status { get; set; }
        
        [Field]
        public Vector3 Location { get; set; }
        
        [Field]
        public byte Face { get; set; }

        public override void HandlePacket()
        {
            switch (Status)
            {
                case PlayerDiggingStatus.StartedDigging:
                    break;
                case PlayerDiggingStatus.CancelledDigging:
                    break;
                case PlayerDiggingStatus.FinishedDigging:
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
                    break;
                case PlayerDiggingStatus.DropItemStack:
                    Client.Player.Inventory.DropCurrentItemStack();
                    break;
                case PlayerDiggingStatus.DropItem:
                    Client.Player.Inventory.DropCurrentItem();
                    break;
                case PlayerDiggingStatus.ShootArrowORFinishEating:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}