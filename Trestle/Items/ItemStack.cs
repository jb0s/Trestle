using Trestle.Enums;
using Trestle.Utils;

namespace Trestle.Items
{
    public struct ItemStack
    {
        public static ItemStack Empty => new(-1);

        public short ItemId;
        public byte ItemCount;
        public short ItemDamage;
        public byte Metadata;
        public byte Nbt;

        private ItemStack(short itemId)
        {
            ItemId = itemId;
            ItemCount = 0;
            ItemDamage = 0;
            Metadata = 0;
            Nbt = 0;
        }

        public ItemStack(Material material)
        {
            ItemId = (short)material;
            ItemCount = 0;
            ItemDamage = 0;
            Metadata = 0;
            Nbt = 0;
        }
        
        public ItemStack(Material material, byte itemCount)
        {
            ItemId = (short)material;
            ItemCount = itemCount;
            ItemDamage = 0;
            Metadata = 0;
            Nbt = 0;
        }
        
        public ItemStack(Material material, byte itemCount, short itemDamage)
        {
            ItemId = (short)material;
            ItemCount = itemCount;
            ItemDamage = itemDamage;
            Metadata = 0;
            Nbt = 0;
        }
        
        public ItemStack(Material material, byte itemCount, byte metadata)
        {
            ItemId = (short)material;
            ItemCount = itemCount;
            ItemDamage = 0;
            Metadata = metadata;
            Nbt = 0;
        }
        
        public ItemStack(Item item, byte itemCount)
        {
            ItemId = (short)item.Id;
            ItemCount = itemCount;
            ItemDamage = 0;
            Metadata = item.Metadata;
            Nbt = 0;
        }
        
        public ItemStack(short itemId, byte itemCount, byte metadata)
        {
            ItemId = itemId;
            ItemCount = itemCount;
            ItemDamage = 0;
            Metadata = metadata;
            Nbt = 0;
        }
        
        public ItemStack(short itemId, byte itemCount, short itemDamage, byte metadata)
        {
            ItemId = itemId;
            ItemCount = itemCount;
            ItemDamage = itemDamage;
            Metadata = metadata;
            Nbt = 0;
        }

        public static bool operator ==(ItemStack a, ItemStack b)
            => a.ItemId == b.ItemId && a.ItemCount == b.ItemCount && a.ItemDamage == b.ItemDamage && a.Metadata == b.Metadata && a.Nbt == b.Nbt;

        public static bool operator !=(ItemStack a, ItemStack b)
            => !(a == b);
    }
}