using Trestle.Attributes;
using Trestle.Enums.Packets.Client;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.EntityHeadLook)]
    public class EntityHeadLook : Packet
    {
        [Field]
        [VarInt]
        public int EntityId { get; set; }
        
        [Field]
        public byte HeadYaw { get; set; }

        public EntityHeadLook(int entityId, byte headYaw)
        {
            EntityId = entityId;
            HeadYaw = headYaw;
        }
    }
}