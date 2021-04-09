using System;
using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Enums.Packets.Client;
using Trestle.Items;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.SetSlot)]
    public class SetSlot : Packet
    {
        [Field]
        public byte WindowId { get; set; }
        
        [Field]
        public short Slot { get; set; }
        
        [Field]
        public ItemStack ItemData { get; set; }

        public SetSlot(byte windowId, short slot, ItemStack item)
        {
            WindowId = windowId;
            Slot = slot;

            ItemData = item;
        }
    }
} 