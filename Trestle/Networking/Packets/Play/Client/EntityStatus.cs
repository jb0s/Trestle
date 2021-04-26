using Trestle.Attributes;
using Trestle.Enums.Packets.Client;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.EntityState)]
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