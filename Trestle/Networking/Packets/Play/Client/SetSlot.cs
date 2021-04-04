using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.Client_SetSlot)]
    public class SetSlot : Packet
    {
        [Field]
        public byte WindowId { get; set; }
        
        [Field]
        public short Slot { get; set; }
        
        [Field]
        public byte[] ItemData { get; set; }

        public SetSlot(byte windowId, short slot, ItemStack item)
        {
            WindowId = windowId;
            Slot = slot;
            
            using (MinecraftStream stream = new MinecraftStream())
            {
                stream.WriteShort(item.ItemId);
                if (item.ItemId != -1)
                {
                    stream.WriteByte(item.ItemCount);
                    stream.WriteShort(item.Metadata);
                    stream.WriteByte(0);
                }

                ItemData = stream.Data;
            }
        }
    }
}