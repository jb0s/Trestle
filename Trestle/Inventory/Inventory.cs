using System;
using System.Diagnostics;
using System.Transactions;
using Trestle.Entity;
using Trestle.Enums;
using Trestle.Items;
using Trestle.Networking.Packets.Play.Client;
using Trestle.Utils;
using Trestle.Worlds;

namespace Trestle.Inventory
{
    public class Inventory
    {
        /// <summary>
        /// Player interacting with the Inventory.
        /// </summary>
        public Player Player;

        /// <summary>
        /// Array of inventory slots.
        /// </summary>
        public ItemStack[] Slots;

        /// <summary>
        /// Window Id of the inventory.
        /// </summary>
        public byte WindowId;

        /// <summary>
        /// Item that was last clicked by the player (used for the ClickWindow packet)
        /// </summary>
        public ItemStack ClickedItem;

        /// <summary>
        /// Is the player dragging in the inventory.
        /// </summary>
        public bool IsDragging;
        
        /// <summary>
        /// The current slot number.
        /// </summary>
        public short HotbarSlot = 0;

        /// <summary>
        /// The current inventory slot that's equipped.
        /// (HotbarSlot + 36)
        /// </summary>
        public short InventorySlot => (short)(HotbarSlot + 36);
        
        /// <summary>
        /// Item that is currently held by the player.
        /// </summary>
        public ItemStack CurrentItem => Slots[InventorySlot];

        /// <summary>
        /// Get an item out of the inventory by its slot index.
        /// </summary>
        /// <param name="index"></param>
        public ItemStack this[int index]
        {
            get => Slots[index];
            set => Slots[index] = value;
        }
        
        public Inventory(Player player, byte windowId, int slotLength)
        {
            Player = player;
            WindowId = windowId;
            
            Slots = new ItemStack[slotLength];
            
            for(var i = 0; i < Slots.Length; i++)
                Slots[i] = new ItemStack(-1, 0, 0);
        }
        
        #region Slots
        
        /// <summary>
        /// Sets a slot in the inventory.
        /// </summary>
        /// <param name="slot">The target slot.</param>
        /// <param name="itemId">The ID of the item that should be in the slot.</param>
        /// <param name="itemCount"></param>
        /// <param name="metaData"></param>
        public void SetSlot(int slot, short itemId, byte itemCount = 1, byte metaData = 0)
        {
            Slots[slot] = new ItemStack(itemId, itemCount, metaData);

            if (Player != null && Player.HasSpawned)
            {
                Player.Client.SendPacket(new SetSlot(0, (short)slot, Slots[slot]));
                
                if(slot == InventorySlot)
                    Player.World.BroadcastPacket(new EntityEquipment(Player.EntityId, EntityEquipmentSlot.MainHand, Slots[slot]));
                else
                    switch (slot)
                    {
                        case 5:
                            Player.World.BroadcastPacket(new EntityEquipment(Player.EntityId, EntityEquipmentSlot.Helmet, Slots[slot]));
                            break;
                        
                        case 6:
                            Player.World.BroadcastPacket(new EntityEquipment(Player.EntityId, EntityEquipmentSlot.Chestplate, Slots[slot]));
                            break;
                        
                        case 7:
                            Player.World.BroadcastPacket(new EntityEquipment(Player.EntityId, EntityEquipmentSlot.Leggings, Slots[slot]));
                            break;
                        
                        case 8:
                            Player.World.BroadcastPacket(new EntityEquipment(Player.EntityId, EntityEquipmentSlot.Boots, Slots[slot]));
                            break;
                        
                        case 45:
                            Player.World.BroadcastPacket(new EntityEquipment(Player.EntityId, EntityEquipmentSlot.OffHand, Slots[slot]));
                            break;
                    }
            }
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

        #region Clicked Item
        
        public void ClearClickedItem()
            => ClickedItem = ItemStack.Empty;
        
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

        #region Exporting & importing

        public byte[] Export()
        {
            using (var stream = new MinecraftStream())
            {
                for (var i = 0; i < Slots.Length; i++)
                {
                    var value = Slots[i];
                    stream.WriteShort(value.ItemId);
                    stream.WriteByte(value.ItemCount);
                    stream.WriteShort(value.ItemDamage);
                    stream.WriteByte(0);
                }

                return stream.Data;
            }
        }
        
        public void Import(byte[] data)
        {
            using (var stream = new MinecraftStream(data))
            {
                for (var i = 0; i < Slots.Length; i++)
                {
                    var itemId = stream.ReadShort();
                    var itemCount = stream.ReadByte();
                    var itemDamage = stream.ReadShort();
                    var metadata = stream.ReadByte();
                    
                    Slots[i] = new ItemStack(itemId, (byte)itemCount, itemDamage, (byte)metadata);
                }
            }
        }

        #endregion
    }
}