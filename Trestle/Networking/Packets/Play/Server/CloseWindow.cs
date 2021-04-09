using Trestle.Attributes;
using Trestle.Enums.Packets.Server;

namespace Trestle.Networking.Packets.Play.Server
{
    [ServerBound(PlayPacket.CloseWindow)]
    public class CloseWindow : Packet
    {
        [Field]
        public byte WindowId { get; set; }

        public override void HandlePacket()
        {
            // kinda a dumb packet but whatever
        }
    }
}