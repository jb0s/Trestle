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
        
        [Field]
        [VarInt]
        public int ActionId { get; set; }
        
        [Field]
        [VarInt]
        public int JumpBoost { get; set; }

        // TODO: Add proper implementation for this.
        public override void HandlePacket()
        {
            var action = (EntityActionType)ActionId;
            switch (action)
            {
                case EntityActionType.StartSneaking:
                    Client.Player.Metadata.Status = 0x02;
                    break;
                
                case EntityActionType.StopSneaking:
                    Client.Player.Metadata.Status = 0x00;
                    break;
                
                default:
                    break;
            }
            
            Client.Player.World.BroadcastPacket(new EntityMetadata(Client.Player), Client.Player);
        }
    }
}