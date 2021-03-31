using Trestle.Attributes;
using Trestle.Enums;

namespace Trestle.Networking.Packets.Play.Server
{
    [ServerBound(PlayPacket.Server_Player)]
    public class Player : Packet
    {
        [Field]
        public bool OnGround { get; set; }

        public override void HandlePacket()
        {
            Client.Player.Location.OnGround = OnGround;
        }
    }
}