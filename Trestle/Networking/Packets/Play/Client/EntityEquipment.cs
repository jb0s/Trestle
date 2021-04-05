using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Enums.Packets.Client;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.EntityEquipment)]
    public class EntityEquipment
    {
        [Field]
        [VarInt]
        public int EntityId { get; set; }
        
        [Field]
        public short Slot { get; set; }
        
        
    }
}