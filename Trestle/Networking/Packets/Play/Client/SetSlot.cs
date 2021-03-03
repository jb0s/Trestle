using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Play
{
    [ClientBound(PlayPacket.SetSlot)]
    public class SetSlot : Packet
    {
        [Field] 
        public byte WindowId { get; set; } = 0;
        
        [Field]
        public short Slot { get; set; }
        
        [Field]
        public short ItemId { get; set; }

        [Field]
        public byte ItemCount { get; set; }
        
        [Field]
        public short Metadata { get; set; }

        [Field]
        public byte Extra { get; set; } = 0;
        
        public SetSlot(ItemStack item, short slot)
        {
            Slot = slot;
            ItemId = item.ItemId;

            if (ItemId != -1)
            {
                ItemCount = item.ItemCount;
                Metadata = item.Metadata;
            }
        }
    }
}