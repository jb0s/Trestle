using Trestle.Attributes;
using Trestle.Enums.Packets.Client;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.EntityLook)]
    public class EntityLook : Packet
    {
        [Field]
        [VarInt]
        public int EntityId { get; set; }
        
        [Field]
        public float Yaw { get; set; }
        
        [Field]
        public float Pitch { get; set; }
        
        public EntityLook(int entityId, Location newLocation)
        {
            EntityId = entityId;

            Yaw = newLocation.HeadYaw;
            Pitch = newLocation.Pitch;
        }
    }
}