using System;
using System.Linq;
using Trestle.Entity;
using Trestle.Enums;
using Trestle.Networking.Packets;
using Trestle.Networking.Packets.Play;

namespace Trestle.Utils
{
    public class InventoryManager
    {
        private readonly Player _player;
        private readonly ItemStack[] _slots = new ItemStack[46];
        
        public ItemStack ClickedItem { get; set; }
        public int CurrentSlot { get; set; }

        private Item PrimaryHand { get; set; }
        private Item OffHand { get; set; }

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
            //PrimaryHand = ItemFactory.GetItemById(s.ItemId, s.Metadata);

            s = GetSlot(45);
            //PrimaryHand = ItemFactory.GetItemById(s.ItemId, s.Metadata);
        }
        
        public Item GetItemInHand(PlayerHand hand)
        {
            UpdateHandItems();
            switch (hand)
            {
                
                case PlayerHand.Primary:
                    return PrimaryHand;
                case PlayerHand.Secondary:
                    return OffHand;
            }

            return null;
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
                    /*
                    new SetSlot(_player.Wrapper)
                    {
                        WindowId = 0,
                        ItemId = itemId,
                        ItemCount = itemcount,
                        Metadata = metadata,
                        ItemDamage = 0,
                        Slot = (short) slot
                    }.Write();*/
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
            {
                return _slots[slot];
            }
            throw new IndexOutOfRangeException("Invalid slot: " + slot);
        }

        public void DropCurrentItem()
        {
            //Drop the current hold item
            var slottarget = 36 + CurrentSlot;
            var slot = GetSlot(slottarget);
            if (slot.ItemCount > 1)
            {
                SetSlot(slottarget, slot.ItemId, slot.Metadata, (byte) (slot.ItemCount - 1));
            }
            else
            {
                SetSlot(slottarget, -1, 0, 0);
            }

            if (slot.ItemId != -1)
            {
                new ItemEntity(_player.World, new ItemStack(slot.ItemId, 1, slot.Metadata)) {Location = _player.Location}
                    .SpawnEntity();
            }
        }
        
        public void DropCurrentItemStack()
        {
            /*int slottarget = 36 + CurrentSlot;
            var slot = GetSlot(slottarget);
            if (slot.ItemId != -1)
            {
                for (int i = 0; i <= slot.ItemCount; i++)
                {
                    new ItemEntity(_player.Level, new ItemStack(slot.ItemId, 1, slot.MetaData)) {KnownPosition = _player.KnownPosition}
                        .SpawnEntity();
                }
                SetSlot(slottarget, -1, 0, 0);
            }*/
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
                    _player.Client.SendPacket(new SetSlot(value, i));
                }
            }
        }

        public byte[] GetBytes()
        {
            DataBuffer buffer = new DataBuffer(new byte[0]);
            for (int i = 0; i <= 45; i++)
            {
                var slot = _slots[i];
                buffer.WriteInt(i);
                buffer.WriteShort(slot.ItemId);
                buffer.WriteByte(slot.Metadata);
                buffer.WriteByte(slot.ItemCount);
            }
            return buffer.ExportWriter;
        }

        public void Import(byte[] data)
        {
            DataBuffer buffer = new DataBuffer(data);

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