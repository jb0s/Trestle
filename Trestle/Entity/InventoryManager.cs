using System;
using System.Linq;
using Trestle.Entity;
using Trestle.Enums;
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

            // If debug mode is enabled, give the player some tools for quick debugging.
            if (Config.Debug)
            {
                AddItem(276, 0, 1); // Sword
                AddItem(278, 0, 1); // Pickaxe
                AddItem(279, 0, 1); // Axe
                AddItem(277, 0, 1); // Shovel
                AddItem(1, 0, 64); // Building blocks
            }

            HeldItemChanged(0);
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
        
        public void SetSlot(int slot, short itemId, byte metadata, byte itemCount)
        {
            if (slot <= 45 && slot >= 5)
            {
                _slots[slot] = new ItemStack(itemId, itemCount, metadata);
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

        public bool AddItem(short itemId, byte metadata, byte itemCount = 1)
        {
            // Try quickbars first
            for(int i = 36; i < 44; i++)
            {
                if (_slots[i].ItemId == itemId && _slots[i].Metadata == metadata && _slots[i].ItemCount < 64)
                {
                    var oldslot = _slots[i];
                    if (oldslot.ItemCount + itemCount <= 64)
                    {
                        SetSlot(i, itemId, metadata, (byte) (oldslot.ItemCount + itemCount));
                        return true;
                    }
                    SetSlot(i, itemId, metadata, 64);
                    var remaining = (oldslot.ItemCount + itemCount) - 64;
                    return AddItem(itemId, metadata, (byte) remaining);
                }
            }
            
            for (var i = 9; i <= 45; i++)
            {
                if (_slots[i].ItemId == itemId && _slots[i].Metadata == metadata && _slots[i].ItemCount < 64)
                {
                    var oldslot = _slots[i];
                    if (oldslot.ItemCount + itemCount <= 64)
                    {
                        SetSlot(i, itemId, metadata, (byte) (oldslot.ItemCount + itemCount));
                        return true;
                    }
                    SetSlot(i, itemId, metadata, 64);
                    var remaining = (oldslot.ItemCount + itemCount) - 64;
                    return AddItem(itemId, metadata, (byte) remaining);
                }
            }

            // Try quickbars first
            for (var i = 36; i < 44; i++)
            {
                if (_slots[i].ItemId == -1)
                {
                    SetSlot(i, itemId, metadata, itemCount);
                    return true;
                }
            }
            
            for (var i = 9; i <= 45; i++)
            {
                if (_slots[i].ItemId == -1)
                {
                    SetSlot(i, itemId, metadata, itemCount);
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
                var location = new Location(_player.Location.X, _player.Location.Y + 1.32f, _player.Location.Z);
                var entity = new ItemEntity(_player.World, new ItemStack(slot.ItemId, 1, slot.Metadata))
                {
                    Location = location,
                    PickupDelay = 40
                };
                entity.SpawnEntity();

                // todo: add velocity & checking of velocity for entities
                _player.World.BroadcastPacket(new SpawnObject(entity, location, 2, 1));
                _player.World.BroadcastPacket(new EntityMetadata(entity));
            }
        }
        
        public void DropCurrentItemStack()
        {
            int slotTarget = 36 + CurrentSlot;
            var slot = GetSlot(slotTarget);
            
            if (slot.ItemId != -1)
            {
                SetSlot(slotTarget, -1, 0, 0);

                var location = new Location(_player.Location.X, _player.Location.Y + 1.32f, _player.Location.Z);
                var entity = new ItemEntity(_player.World, new ItemStack(slot.ItemId, slot.ItemCount, slot.Metadata))
                {
                    Location = location,
                    PickupDelay = 40
                };
                entity.SpawnEntity();

                // todo: add velocity & checking of velocity
                _player.World.BroadcastPacket(new SpawnObject(entity, location, 2, 1));
                _player.World.BroadcastPacket(new EntityMetadata(entity));
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