using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Play.Server
{
    [ServerBound(PlayPacket.Server_PlayerPosition)]
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
            Client.Player.Location.X = X;
            Client.Player.Location.Y = Y;
            Client.Player.Location.Z = Z;
            Client.Player.Location.OnGround = OnGround;
        }
    }
}