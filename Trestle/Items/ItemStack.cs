namespace Trestle.Utils
{
    public class ItemStack
    {
        public short ItemId { get; set; }
        public byte ItemCount { get; set; }
        public byte Metadata { get; set; }
        public byte NBT { get; private set; }
        
        public ItemStack(short itemId, byte itemCount, byte metadata)
        {
            ItemId = itemId;
            ItemCount = itemCount;
            Metadata = metadata;
            NBT = 0;
        }
        
        public ItemStack(Item item, byte itemCount)
        {
            ItemId = (short) item.Id;
            ItemCount = itemCount;
            Metadata = item.Metadata;
        }
    }
}