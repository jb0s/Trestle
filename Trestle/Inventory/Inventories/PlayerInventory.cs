using Trestle.Entity;
using Trestle.Enums;
using Trestle.Items;
using Trestle.Networking.Packets.Play.Client;

namespace Trestle.Inventory.Inventories
{
    public class PlayerInventory : Inventory
    {
        public PlayerInventory(Player player) : base(player, 0, 46)
        {
        }
        
        public bool AddItem(short itemId, int itemCount = 1, byte metadata = 0)
        {
            // Try quickbars first
            for(int i = 36; i < 44; i++)
            {
                if (Slots[i].ItemId == itemId && Slots[i].Metadata == metadata && Slots[i].ItemCount < 64)
                {
                    var oldslot = Slots[i];
                    if (oldslot.ItemCount + itemCount <= 64)
                    {
                        SetSlot(i, itemId, oldslot.ItemCount + itemCount, metadata);
                        return true;
                    }
                    
                    SetSlot(i, itemId, 64, metadata);
                    return AddItem(itemId, oldslot.ItemCount + itemCount - 64, metadata);
                }
            }
            
            for (var i = 9; i <= 45; i++)
            {
                if (Slots[i].ItemId == itemId && Slots[i].Metadata == metadata && Slots[i].ItemCount < 64)
                {
                    var oldslot = Slots[i];
                    if (oldslot.ItemCount + itemCount <= 64)
                    {
                        SetSlot(i, itemId, oldslot.ItemCount + itemCount, metadata);
                        return true;
                    }
                    SetSlot(i, itemId, itemCount, metadata);
                    return AddItem(itemId, oldslot.ItemCount + itemCount - 64, metadata);
                }
            }

            // Try quickbars first
            for (var i = 36; i < 44; i++)
            {
                if (Slots[i].ItemId == -1)
                {
                    SetSlot(i, itemId, itemCount, metadata);
                    return true;
                }
            }
            
            for (var i = 9; i <= 45; i++)
            {
                if (Slots[i].ItemId == -1)
                {
                    SetSlot(i, itemId, itemCount, metadata);
                    return true;
                }
            }
            
            return false;
        }

        public bool RemoveItem(short itemId, short count, short metaData)
        {
            for (var i = 0; i <= 45; i++)
            {
                var itemStack = Slots[i];
                if (itemStack.ItemId == itemId && itemStack.Metadata == metaData)
                {
                    if ((itemStack.ItemCount - count) > 0)
                    {
                        SetSlot(i, itemStack.ItemId, itemStack.ItemCount - count, itemStack.Metadata);
                        return true;
                    }
                    
                    SetSlot(i, -1, 0);
                    return true;
                }
            }
            return false;
        }
    }
}