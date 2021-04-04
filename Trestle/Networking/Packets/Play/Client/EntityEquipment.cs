using Trestle.Attributes;
using Trestle.Enums;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.Client_EntityEquipment)]
    public class EntityEquipment
    {
        [Field]
        [VarInt]
        public int EntityId { get; set; }
        
        [Field]
        public short Slot { get; set; }
        
        
    }
}