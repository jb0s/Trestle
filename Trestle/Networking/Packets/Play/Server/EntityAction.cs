using System.Transactions;
using Trestle.Attributes;
using Trestle.Entity;
using Trestle.Enums;
using Trestle.Enums.Packets.Server;
using Trestle.Networking.Packets.Play.Client;

namespace Trestle.Networking.Packets.Play.Server
{
    [ServerBound(PlayPacket.EntityAction)]
    public class EntityAction : Packet
    {
        [Field]
        [VarInt]
        public int EntityId { get; set; }
        
        [Field(typeof(int))]
        [VarInt]
        public EntityActionType ActionId { get; set; }
        
        [Field]
        [VarInt]
        public int JumpBoost { get; set; }

        public override void HandlePacket()
        {
            switch (ActionId)
            {
                case EntityActionType.StartSneaking:
                case EntityActionType.StopSneaking:
                    Client.Player.IsCrouching = ActionId == EntityActionType.StartSneaking;
                    break;
                case EntityActionType.StartSprinting:
                case EntityActionType.StopSprinting:
                    Client.Player.IsSprinting = ActionId == EntityActionType.StartSprinting;
                    break;
            }
        }
    }
}