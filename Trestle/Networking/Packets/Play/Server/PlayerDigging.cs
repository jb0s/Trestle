﻿using System;
using Trestle.Attributes;
using Trestle.Entity;
using Trestle.Enums;
using Trestle.Enums.Packets.Server;
using Trestle.Items;
using Trestle.Networking.Packets.Play.Client;
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
            if (Status == PlayerDiggingStatus.StartedDigging && Client.Player.GameMode == GameMode.Creative)
                Status = PlayerDiggingStatus.FinishedDigging;
            
            switch (Status)
            {
                case PlayerDiggingStatus.StartedDigging:
                    break;
                case PlayerDiggingStatus.CancelledDigging:
                    break;
                case PlayerDiggingStatus.FinishedDigging:
                    var block = Client.Player.World.GetBlock(Location, true);

                    if (Client.Player.GameMode == GameMode.Survival)
                    {
                        new ItemEntity(Client.Player.World, new ItemStack(new Item(block.Id, 0), 1))
                        {
                            Location = new Location(Location.X, Location.Y, Location.Z)
                        }.SpawnEntity();
                    }
                    
                    block.BreakBlock(Client.Player.World);
                    break;
                case PlayerDiggingStatus.DropItemStack:
                   // Client.Player.Inventory.DropCurrentItemStack();
                    break;
                case PlayerDiggingStatus.DropItem:
                   // Client.Player.Inventory.DropCurrentItem();
                    break;
                case PlayerDiggingStatus.ShootArrow:
                    break;
                case PlayerDiggingStatus.SwapItemInHand:
                    break;
            }
        }
    }
}