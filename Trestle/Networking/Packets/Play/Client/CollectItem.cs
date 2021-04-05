using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Enums.Packets.Client;

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

        public CollectItem(Entity.Entity collected, Entity.Entity collector)
        {
            CollectedEntityId = collected.EntityId;
            CollectorEntityId = collector.EntityId;
        }
    }
}