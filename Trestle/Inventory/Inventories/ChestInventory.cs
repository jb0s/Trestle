using Trestle.Entity;
using Trestle.Items;

namespace Trestle.Inventory.Inventories
{
    public class ChestInventory : Inventory
    {
        public ChestInventory(Player player, byte windowId, bool isLargeChest) : base(player, windowId, isLargeChest ? 90 : 63)
        {
        }
    }
}