using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Enums.Packets.Client;
using Trestle.Items;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.EntityEquipment)]
    public class EntityEquipment : Packet
    {
        [Field]
        [VarInt]
        public int EntityId { get; set; }
        
        [Field(typeof(int))]
        [VarInt]
        public EntityEquipmentSlot Slot { get; set; }
        
        [Field]
        public ItemStack Item { get; set; }

        public EntityEquipment(int entityId, EntityEquipmentSlot slot, ItemStack item)
        {
            EntityId = entityId;
            Slot = slot;
            Item = item;
        }
    }
}