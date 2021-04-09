using System;
using System.Threading;
using Trestle.Items;
using Trestle.Networking.Packets.Play.Client;
using Trestle.Utils;

namespace Trestle.Entity
{
    public class InventoryManager
    {
        /// <summary>
        /// Owner of the inventory.
        /// </summary>
        public Player Player { get; }
        

        /// <summary>
        /// Array of inventory slots.
        /// </summary>
        public ItemStack[] Slots = new ItemStack[46];

        /// <summary>
        /// Item that was last clicked by the player (used for the ClickWindow packet)
        /// </summary>
        public ItemStack ClickedItem { get; set; }

        /// <summary>
        /// Is the player dragging in the inventory.
        /// </summary>
        public bool IsDragging { get; set; }
        
        /// <summary>
        /// The current slot number.
        /// </summary>
        public short CurrentSlot { get; set; } = 0;
        
        /// <summary>
        /// Item that is currently held by the player.
        /// </summary>
        public ItemStack CurrentItem => Slots[CurrentSlot + 36];

        public ItemStack this[int index]
        {
            get => Slots[index];
            set => Slots[index] = value;
        }
        
        public InventoryManager(Player player)
        {
            Player = player;
            
            for(var i = 0; i < Slots.Length; i++)
                Slots[i] = new ItemStack(-1, 0, 0);
        }
        
        #region Slots
        
        public void SetSlot(int slot, short itemId, byte itemCount = 1, byte metaData = 0)
        {
            Slots[slot] = new ItemStack(itemId, itemCount, metaData);
            
            if (Player != null && Player.HasSpawned)
                Player.Client.SendPacket(new SetSlot(0, (short)slot, Slots[slot]));
        }

        public void SetSlot(int slot, short itemId, int itemCount = 1, byte metaData = 0)
            => SetSlot(slot, itemId, (byte)itemCount, metaData);
        
        public void SetSlot(int slot, ItemStack itemStack, bool sendPacket = true)
            => SetSlot(slot, itemStack.ItemId, itemStack.ItemCount, itemStack.Metadata);

        public void SetSlotItemCount(int slot, int itemCount)
        {
            var itemStack = Slots[slot];
            
            if (itemCount == 0)
                SetSlot(slot, -1, 0);
            else
                SetSlot(slot, itemStack.ItemId, itemCount, itemStack.Metadata);
        }

        public void ClearSlot(int slot)
        {
            Slots[slot] = new ItemStack(-1, 0, 0);
        }
        
        #endregion

        #region Items
        
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

        #endregion
        
        #region Clicked Item
        
        public void ClearClickedItem()
            => ClickedItem = null;
        
        public void SendToPlayer()
        {
            for (short i = 0; i <= 45; i++)
            {
                var value = Slots[i];
                if (value.ItemId != -1)
                    Player.Client.SendPacket(new SetSlot(0, i, value));
            }
        }

        #endregion
    }
}