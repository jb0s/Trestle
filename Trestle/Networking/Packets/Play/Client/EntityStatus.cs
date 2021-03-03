using Trestle.Attributes;
using Trestle.Enums;

namespace Trestle.Networking.Packets.Play
{
    [ClientBound(PlayPacket.EntityStatus)]
    public class EntityStatus : Packet
    {
        [Field]
        public int EntityId { get; set; }
        
        [Field]
        public byte Status { get; set; }

        public EntityStatus(int entityId, byte status)
        {
            EntityId = entityId;
            Status = status;
        }
    }
}