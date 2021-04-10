using System;
using Trestle.Entity;
using Trestle.Enums;
using Trestle.Inventory.Inventories;
using Trestle.Networking.Packets.Play.Client;
using Trestle.Utils;

namespace Trestle.Block.Blocks
{
    public class Chest : Block
    {
        public Chest() : base(Material.Chest)
        {
            IsUsable = true;
        }

        public override void UseItem(World.World world, Player player, Vector3 blockCoordinates, BlockFace face)
        {
            var windowId = (byte)(player.World.Windows.Count + 1);
            player.World.Windows[windowId] = new ChestInventory(player, windowId, false);
            
            player.Client.SendPacket(new OpenWindow(windowId, WindowType.Chest, new MessageComponent("Chest"), 27));
        }
    }
}