using Trestle.Utils;
using Trestle.Attributes;
using Trestle.Enums.Packets.Client;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.EntityVelocity)]
    public class EntityVelocity : Packet
    {
        [Field]
        [VarInt]
        public int EntityId { get; set; }
        
        [Field]
        public short VelocityX { get; set; }
        
        [Field]
        public short VelocityY { get; set; }
        
        [Field]
        public short VelocityZ { get; set; }

        public EntityVelocity(int entityId, Vector3 velocity)
        {
            EntityId = entityId;

            VelocityX = (short)velocity.X;
            VelocityY = (short)velocity.Y;
            VelocityZ = (short)velocity.Z;
        }
    }
}