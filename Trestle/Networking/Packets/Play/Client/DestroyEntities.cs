using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Enums.Packets.Client;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.DestroyEntities)]
    public class DestroyEntities : Packet
    {
        [Field]
        [VarInt]
        public int Count { get; set; }
        
        [Field]
        [VarInt]
        public int[] EntityIds { get; set; }

        public DestroyEntities(int[] entityIds)
        {
            Count = entityIds.Length;
            EntityIds = entityIds;
        }
    }
}