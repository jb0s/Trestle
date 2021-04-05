using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Enums.Packets.Server;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Play.Server
{
    [ServerBound(PlayPacket.PlayerPosition)]
    public class PlayerPosition : Packet
    {
        [Field]
        public double X { get; set; }
        
        [Field]
        public double Y { get; set; }
        
        [Field]
        public double Z { get; set; }
        
        [Field]
        public bool OnGround { get; set; }

        public override void HandlePacket()
        {
            Entity.Player player = Client.Player;

            Vector3 newLocation = new Vector3(X, Y, Z);
            player.PositionChanged(newLocation, player.Location.Yaw, player.Location.Pitch, OnGround);
        }
    }
}