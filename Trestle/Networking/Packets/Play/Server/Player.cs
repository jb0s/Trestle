using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Enums.Packets.Server;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Play.Server
{
    [ServerBound(PlayPacket.Player)]
    public class Player : Packet
    {
        [Field]
        public bool OnGround { get; set; }

        public override void HandlePacket()
        {
            Entity.Player player = Client.Player;
            player.PositionChanged(player.Location.ToVector3(), player.Location.Yaw, player.Location.Pitch, OnGround);
        }
    }
}