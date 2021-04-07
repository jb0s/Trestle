using System;
using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Enums.Packets.Server;

namespace Trestle.Networking.Packets.Play.Server
{
    [ServerBound(PlayPacket.CreativeInventoryAction)]
    public class CreativeInventoryAction : Packet
    {
        [Field]
        public short Slot { get; set; }
        
        [Field]
        public short BlockId { get; set; }
        
        [Field]
        [Optional]
        public byte ItemCount { get; set; }
        
        [Field]
        [Optional]
        public short ItemDamage { get; set; }

        public override void HandlePacket()
        {
            // Sanity check, if this is sent without creative someones probably messing with packets.
            if (Client.Player.GameMode != GameMode.Creative)
                return;
            
            Client.Player.Inventory.SetSlot(Slot, BlockId, 0, ItemCount);
        }
    }
}