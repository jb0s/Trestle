using Trestle.Attributes;
using Trestle.Enums.Packets.Client;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.EntityTeleport)]
    public class EntityTeleport : Packet
    {
        [Field]
        [VarInt]
        public int EntityId { get; set; }
        
        [Field]
        public Location Location { get; set; }

        public EntityTeleport(int entityId, Location location)
        {
            EntityId = entityId;
            Location = location;
        }
    }
}