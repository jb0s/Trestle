using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Enums.Packets.Client;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.Animation)]
    public class Animation : Packet
    {
        [Field]
        [VarInt]
        public int EntityId { get; set; }
        
        [Field]
        public byte AnimationId { get; set; }

        public Animation(Entity.Entity entity, AnimationType animationType)
        {
            EntityId = entity.EntityId;
            AnimationId = (byte)animationType;
        }
    }
}