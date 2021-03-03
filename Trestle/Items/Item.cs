using System;
using System.Collections;
using Trestle.Entity;
using Trestle.Enums;
using Trestle.Items;
using Trestle.Worlds;
using ItemType = Trestle.Items.ItemType;

namespace Trestle.Utils
{
    public class Item
    {
        private short _fuelEfficiency;
        public ushort Id { get; set; }
        public ToolMaterial ToolMaterial { get; set; }
        public ItemType ItemType { get; set; }
        public byte Metadata { get; set; }
        public bool IsUsable { get; set; }
        public int MaxStackSize { get; set; }
        public ItemStack[] CraftingItems { get; set; }
        public bool IsBlock { get; set; }
        public Material Material
        {
            get => (Material)Id;
            set => Id = (ushort)value;
        }
        
        // We have to do this because { get; protected set; } doesn't exist :(
        protected short FuelEfficiency
        {
            set => _fuelEfficiency = value;
        }
        
        public Item() {}
        
        internal Item(ushort id, byte metadata)
        {
            Id = id;
            Metadata = metadata;

            ToolMaterial = ToolMaterial.Air;
            ItemType = ItemType.Item;
            IsUsable = false;
            MaxStackSize = 64;
            IsBlock = false;
        }
        
        protected Item(ushort id, byte metadata, short fuelEfficiency) : this(id, metadata)
        {
            FuelEfficiency = fuelEfficiency;
        }
        
        public virtual void UseItem(World world, Player player, Vector3 blockCoordinates, BlockFace face)
        {
        }
        
        protected Vector3 GetNewCoordinatesFromFace(Vector3 target, BlockFace face)
        {
            switch (face)
            {
                case BlockFace.NegativeY:
                    target.Y--;
                    break;
                case BlockFace.PositiveY:
                    target.Y++;
                    break;
                case BlockFace.NegativeZ:
                    target.Z--;
                    break;
                case BlockFace.PositiveZ:
                    target.Z++;
                    break;
                case BlockFace.NegativeX:
                    target.X--;
                    break;
                case BlockFace.PositiveX:
                    target.X++;
                    break;
            }
            return target;
        }
        
        public int GetDamage()
        {
            switch (ItemType)
            {
                case ItemType.Sword:
                    return GetSwordDamage(ToolMaterial);
                case ItemType.Axe:
                    return GetAxeDamage(ToolMaterial);
                case ItemType.Pickaxe:
                    return GetPickAxeDamage(ToolMaterial);
                case ItemType.Shovel:
                    return GetShovelDamage(ToolMaterial);
                default:
                    return 1;
            }
        }
        
        protected int GetSwordDamage(ToolMaterial itemToolMaterial)
        {
            switch (itemToolMaterial)
            {
                case ToolMaterial.Gold:
                case ToolMaterial.Wood:
                    return 5;
                case ToolMaterial.Stone:
                    return 6;
                case ToolMaterial.Iron:
                    return 7;
                case ToolMaterial.Diamond:
                    return 8;
                default:
                    return 1;
            }
        }
        
        private int GetAxeDamage(ToolMaterial itemToolMaterial)
        {
            return GetSwordDamage(itemToolMaterial) - 1;
        }

        private int GetPickAxeDamage(ToolMaterial itemToolMaterial)
        {
            return GetSwordDamage(itemToolMaterial) - 2;
        }

        private int GetShovelDamage(ToolMaterial itemToolMaterial)
        {
            return GetSwordDamage(itemToolMaterial) - 3;
        }
        
        public virtual short GetFuelEfficiency()
        {
            return _fuelEfficiency;
        }

        public virtual Item GetSmelt()
        {
            return null;
        }

        public ItemStack GetItemStack()
        {
            return new ItemStack((short)Id, 1, Metadata);
        }
        
        public byte ConvertToByte(BitArray bits)
        {
            if (bits.Count != 8)
            {
                throw new ArgumentException("bits");
            }
            byte[] bytes = new byte[1];
            bits.CopyTo(bytes, 0);
            return bytes[0];
        }
    }
}