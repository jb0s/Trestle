using Trestle.Attributes;
using Trestle.Entity;
using Trestle.Enums;
using Trestle.Enums.Packets.Client;
using Trestle.Items;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.CollectItem)]
    public class CollectItem : Packet
    {
        [Field]
        [VarInt]
        public int CollectedEntityId { get; set; }
        
        [Field]
        [VarInt]
        public int CollectorEntityId { get; set; }
        
        [Field]
        [VarInt]
        public int PickupItemCount { get; set; }

        public CollectItem(ItemEntity collected, Entity.Entity collector)
        {
            CollectedEntityId = collected.EntityId;
            CollectorEntityId = collector.EntityId;
            PickupItemCount = collected.Item.ItemCount;
        }
    }
}