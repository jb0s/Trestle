namespace Trestle.Utils
{
    public class Slot
    {
        public short BlockId { get; set; } = 0x00;
        
        public byte ItemCount { get; set; } = 0x00;
        
        public short ItemDamage { get; set; } = 0x00;
        
        public byte NBT { get; set; } = 0x00;

        public Slot(ItemStack item)
        {
            BlockId = item.ItemId;
            ItemCount = item.ItemCount;
            ItemDamage = 0x00;
        }
    }
}