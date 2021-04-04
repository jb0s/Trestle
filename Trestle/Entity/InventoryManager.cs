using System;
using System.Linq;
using Trestle.Entity;
using Trestle.Networking.Packets.Play.Client;

namespace Trestle.Utils
{
    public class InventoryManager
    {
        private readonly Player _player;
        private readonly ItemStack[] _slots = new ItemStack[46];
        
        public ItemStack ClickedItem { get; set; }
        public int CurrentSlot { get; set; }

        private Item Hand { get; set; }

        public InventoryManager(Player player)
        {
            _player = player;
            
            for(var i = 0; i < _slots.Length; i++)
                _slots[i] = new ItemStack(-1, 0, 0);

            UpdateHandItems();
        }
        
        public void InventoryClosed()
        {
            if (ClickedItem != null)
            {
                AddItem(ClickedItem);
                ClickedItem = null;
            }
        }
        
        public void HeldItemChanged(int newSlot)
        {
            CurrentSlot = newSlot;
            UpdateHandItems();
        }
        
        private void UpdateHandItems()
        {
            var s = GetSlot(CurrentSlot + 36);
            Hand = new Item((ushort)s.ItemId, s.Metadata);
        }
        
        public Item GetItemInHand()
        {
            UpdateHandItems();
            return Hand;
        }

        public void SwapHands()
        {
            UpdateHandItems();
            var primaryHand = GetSlot(CurrentSlot + 36);
            var offHand = GetSlot(45);
            SetSlot(CurrentSlot + 36, offHand.ItemId, offHand.Metadata, offHand.ItemCount);
            SetSlot(45, primaryHand.ItemId, primaryHand.Metadata, primaryHand.ItemCount);
        }
        
        public bool HasItems(ItemStack[] items)
        {
            foreach (var item in items)
            {
                if (!HasItem(item.ItemId)) return false;
            }
            return true;
        }
        
        public void SetSlot(int slot, short itemId, byte metadata, byte itemcount)
        {
            if (slot <= 45 && slot >= 5)
            {
                _slots[slot] = new ItemStack(itemId, itemcount, metadata);
                if (_player != null && _player.HasSpawned)
                {
                    _player.Client.SendPacket(new SetSlot(0, (short)slot, _slots[slot]));
                }
            }
            UpdateHandItems();
        }
        
        public bool AddItem(ItemStack item)
        {
            return AddItem(item.ItemId, item.Metadata, item.ItemCount);
        }

        public bool AddItem(short itemId, byte metadata, byte itemcount = 1)
        {
            // Try quickbars first
            for(int i = 36; i < 44; i++)
            {
                if (_slots[i].ItemId == itemId && _slots[i].Metadata == metadata && _slots[i].ItemCount < 64)
                {
                    var oldslot = _slots[i];
                    if (oldslot.ItemCount + itemcount <= 64)
                    {
                        SetSlot(i, itemId, metadata, (byte) (oldslot.ItemCount + itemcount));
                        return true;
                    }
                    SetSlot(i, itemId, metadata, 64);
                    var remaining = (oldslot.ItemCount + itemcount) - 64;
                    return AddItem(itemId, metadata, (byte) remaining);
                }
            }
            
            for (var i = 9; i <= 45; i++)
            {
                if (_slots[i].ItemId == itemId && _slots[i].Metadata == metadata && _slots[i].ItemCount < 64)
                {
                    var oldslot = _slots[i];
                    if (oldslot.ItemCount + itemcount <= 64)
                    {
                        SetSlot(i, itemId, metadata, (byte) (oldslot.ItemCount + itemcount));
                        return true;
                    }
                    SetSlot(i, itemId, metadata, 64);
                    var remaining = (oldslot.ItemCount + itemcount) - 64;
                    return AddItem(itemId, metadata, (byte) remaining);
                }
            }

            // Try quickbars first
            for (var i = 36; i < 44; i++)
            {
                if (_slots[i].ItemId == -1)
                {
                    SetSlot(i, itemId, metadata, itemcount);
                    return true;
                }
            }
            
            for (var i = 9; i <= 45; i++)
            {
                if (_slots[i].ItemId == -1)
                {
                    SetSlot(i, itemId, metadata, itemcount);
                    return true;
                }
            }
            return false;
        }
        
        public ItemStack GetSlot(int slot)
        {
            if (slot <= 45 && slot >= 0)
                return _slots[slot];
            
            throw new IndexOutOfRangeException("Invalid slot: " + slot);
        }

        public void DropCurrentItem()
        {
            //Drop the current hold item
            var slotTarget = 36 + CurrentSlot;
            var slot = GetSlot(slotTarget);
            if (slot.ItemCount > 1)
                SetSlot(slotTarget, slot.ItemId, slot.Metadata, (byte) (slot.ItemCount - 1));
            else
                SetSlot(slotTarget, -1, 0, 0);

            if (slot.ItemId != -1)
            {
                var item = new ItemEntity(_player.World, new ItemStack(slot.ItemId, 1, slot.Metadata))
                {
                    PickupDelay = 40
                };

                var f = 0.3f;
                var f1 = Math.Sin(_player.Location.Pitch * 0.017453292f);
                var f2 = Math.Cos(_player.Location.Pitch * 0.017453292f);
                var f3 = Math.Sin(_player.Location.Yaw * 0.017453292f);
                var f4 = Math.Cos(_player.Location.Yaw * 0.017453292f);
                var f5 = Globals.Random.NextDouble() * 6.2831855F;
                var f6 = 0.02f * Globals.Random.NextDouble();

                item.Location = new Location(-f3 * f2 * 0.3f + Math.Cos(f5) * f6, 
                    -f1 * 0.3f * 0.1f + (Globals.Random.NextDouble() - Globals.Random.NextDouble()), 
                    f4 * f2 * 0.3f + Math.Sin(f5) * f6);
                
                item.SpawnEntity();
            }
        }
        
        public void DropCurrentItemStack()
        {
            int slotTarget = 36 + CurrentSlot;
            var slot = GetSlot(slotTarget);
            if (slot.ItemId != -1)
            {
                for (int i = 0; i <= slot.ItemCount; i++)
                {
                    new ItemEntity(_player.World, new ItemStack(slot.ItemId, 1, slot.Metadata)) {Location = _player.Location}
                        .SpawnEntity();
                }
                SetSlot(slotTarget, -1, 0, 0);
            }
        }
        
        public bool HasItem(int itemId)
        {
            if (_slots.Any(itemStack => itemStack.ItemId == itemId))
            {
                return true;
            }
            return false;
        }

        public bool RemoveItem(short itemId, short metaData, short count)
        {
            for (var index = 0; index <= 45; index++)
            {
                var itemStack = _slots[index];
                if (itemStack.ItemId == itemId && itemStack.Metadata == metaData && itemStack.ItemCount >= count)
                {
                    if ((itemStack.ItemCount - count) > 0)
                    {
                        SetSlot(index, itemStack.ItemId, itemStack.Metadata, (byte) (itemStack.ItemCount - count));
                        return true;
                    }
                    SetSlot(index, -1, 0, 0);
                    return true;
                }
            }
            return false;
        }

        public void SendToPlayer()
        {
            for (short i = 0; i <= 45; i++)
            {
                var value = _slots[i];
                if (value.ItemId != -1)
                {
                    _player.Client.SendPacket(new SetSlot(0, i, value));
                }
            }
        }

        public byte[] GetBytes()
        {
            var buffer = new MinecraftStream();
            for (int i = 0; i <= 45; i++)
            {
                var slot = _slots[i];
                buffer.WriteInt(i);
                buffer.WriteShort(slot.ItemId);
                buffer.WriteByte(slot.Metadata);
                buffer.WriteByte(slot.ItemCount);
            }
            return buffer.Data;
        }

        public void Import(byte[] data)
        {
            var buffer = new MinecraftStream(data);

            for (int i = 0; i <= 45; i++)
            {
                int slotId = buffer.ReadInt();
                short itemId = buffer.ReadShort();
                byte metaData = (byte)buffer.ReadByte();
                byte itemCount = (byte)buffer.ReadByte();

                _slots[slotId] = new ItemStack(itemId, itemCount, metaData);
                UpdateHandItems();
            }
        }
    }
}