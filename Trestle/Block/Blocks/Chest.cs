using System;
using fNbt.Tags;
using Trestle.Entity;
using Trestle.Enums;
using Trestle.Inventory.Inventories;
using Trestle.Networking.Packets.Play.Client;
using Trestle.Utils;
using Trestle.World;

namespace Trestle.Block.Blocks
{
    public class Chest : Block
    {
        public Chest() : base(Material.Chest)
        {
            IsUsable = true;
        }

        public override void UseItem(Worlds.World world, Player player, Vector3 blockCoordinates, BlockFace face)
        {
            var windowId = (byte)(player.World.Windows.Count + 1);
            player.World.Windows[windowId] = new ChestInventory(player, windowId, false);
            /*
            player.World.SetBlockEntity(blockCoordinates, new BlockEntity(BlockEntityType.Chest, blockCoordinates, new NbtCompound()
            {
                new NbtList("Items")
                {
                    new NbtCompound()
                    {
                        new NbtByte("Count", 1),
                        new NbtByte("Slot", 0),
                        new NbtString("id","1"),
                        new NbtCompound("tag")
                    }
                }
            }));
            player.World.Save();
            player.World.GetBlockEntity(blockCoordinates);*/
            
            player.Client.SendPacket(new OpenWindow(windowId, WindowType.Chest, new MessageComponent("Chest"), 27));
        }
    }
}