using Trestle.Attributes;
using Trestle.Enums;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.Client_EntityMetadata)]
    public class EntityMetadata : Packet
    {
        [Field]
        [VarInt]
        public int EntityId { get; set; }
        
        [Field]
        public byte[] Data { get; set; }

        public EntityMetadata(int entityId, byte[] data)
        {
            EntityId = entityId;
            Data = data;
        }
    }
}