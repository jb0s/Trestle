using Trestle.Attributes;
using Trestle.Enums.Packets.Client;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.EntityRelativeMove)]
    public class EntityRelativeMove : Packet
    {
        [Field]
        [VarInt]
        public int EntityId { get; set; }
        
        [Field]
        public short DeltaX { get; set; }
        
        [Field]
        public short DeltaY { get; set; }
        
        [Field]
        public short DeltaZ { get; set; }
        
        public EntityRelativeMove(int entityId, Location prevLocation, Location newLocation)
        {
            EntityId = entityId;

            DeltaX = (short)((newLocation.X * 32 - prevLocation.X * 32) * 128);
            DeltaY = (short)((newLocation.Y * 32 - prevLocation.Y * 32) * 128);
            DeltaZ = (short)((newLocation.Z * 32 - prevLocation.Z * 32) * 128);
        }
    }
}