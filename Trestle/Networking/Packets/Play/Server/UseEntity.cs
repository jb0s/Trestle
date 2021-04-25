using System.Numerics;
using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Enums.Packets.Server;

namespace Trestle.Networking.Packets.Play.Server
{
    [ServerBound(PlayPacket.UseEntity)]
    public class UseEntity : Packet
    {
        [Field]
        [VarInt]
        public int Target { get; set; }
        
        [Field(typeof(int))]
        [VarInt]
        public InteractionType Type { get; set; }

        public override void HandlePacket()
        {
            switch (Type)
            {
                case InteractionType.Attack:
                    if(World.Players.ContainsKey(Target))
                        Player.World.Players[Target].HealthManager.Pain(1);
                    else
                        Player.World.Entities[Target].HealthManager.Pain(1);
                    break;
            }
        }
    }
}