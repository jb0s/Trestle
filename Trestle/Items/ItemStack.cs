using Trestle.Utils;

namespace Trestle.Items
{
    public class ItemStack
    {
        public short ItemId { get; set; }
        public byte ItemCount { get; set; }
        public short ItemDamage { get; set; }
        public byte Metadata { get; set; }
        public byte NBT { get; private set; }
        
        public ItemStack(Item item, byte itemCount)
        {
            ItemId = (short)item.Id;
            ItemCount = itemCount;
            ItemDamage = 0;
            Metadata = item.Metadata;
        }
        
        public ItemStack(short itemId, byte itemCount, byte metadata)
        {
            ItemId = itemId;
            ItemCount = itemCount;
            ItemDamage = 0;
            Metadata = metadata;
            NBT = 0;
        }
        
        public ItemStack(short itemId, byte itemCount, short itemDamage, byte metadata)
        {
            ItemId = itemId;
            ItemCount = itemCount;
            ItemDamage = itemDamage;
            Metadata = metadata;
            NBT = 0;
        }
    }
}