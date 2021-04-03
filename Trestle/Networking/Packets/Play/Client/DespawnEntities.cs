using Trestle.Attributes;
using Trestle.Enums;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.Client_DestroyEntities)]
    public class DespawnEntities : Packet
    {
        [Field]
        [VarInt]
        public int Count { get; set; }
        
        [Field]
        [VarInt]
        public int[] EntityIds { get; set; }

        public DespawnEntities(int[] entityIds)
        {
            Count = entityIds.Length;
            EntityIds = entityIds;
        }
    }
}