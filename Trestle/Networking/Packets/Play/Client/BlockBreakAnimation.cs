using Trestle.Attributes;
using Trestle.Enums.Packets.Client;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.BlockBreakAnimation)]
    public class BlockBreakAnimation : Packet
    {
        [Field]
        [VarInt]
        public int EntityId { get; set; }
        
        [Field]
        public Vector3 Location { get; set; }
        
        [Field]
        public byte DestroyStage { get; set; }

        public BlockBreakAnimation(Entity.Entity entity, Vector3 location, byte stage)
        {
            EntityId = entity.EntityId;
            Location = location;
            DestroyStage = stage;
        }
    }
}