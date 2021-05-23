using System.Threading.Tasks;
using Trestle.Networking.Attributes;
using Trestle.Networking.Enums.Server;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Play.Server
{
    [ServerBound(PlayPacket.PlayerPosition)]
    public class PlayerPosition : Packet
    {
        [Field]
        public double X { get; set; }
        
        [Field]
        public double FeetY { get; set; }
        
        [Field]
        public double Z { get; set; }
        
        [Field]
        public bool OnGround { get; set; }

        public override Task Handle()
        {
            Client.Player.Location = new Location(X, FeetY, Z);
            
            return Task.CompletedTask;
        }
    }
}