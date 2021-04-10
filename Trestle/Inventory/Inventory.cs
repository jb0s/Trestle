using System;
using Trestle.Entity;
using Trestle.Items;
using Trestle.Networking.Packets.Play.Client;
using Trestle.Utils;

namespace Trestle.Inventory
{
    public class Inventory
    {
        /// <summary>
        /// Player interacting with the Inventory.
        /// </summary>
        public Player Player { get; }
        
        /// <summary>
        /// Array of inventory slots.
        /// </summary>
        public ItemStack[] Slots { get; set; }
        
        /// <summary>
        /// Window Id of the inventory.
        /// </summary>
        public byte WindowId { get; set; }

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
        
        public Inventory(Player player, byte windowId, int slotLength)
        {
            Player = player;
            WindowId = windowId;
            
            Slots = new ItemStack[slotLength];
            
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