using Trestle.Attributes;
using Trestle.Enums;

namespace Trestle.Networking.Packets.Status
{
    [ClientBound(StatusPacket.Pong)]
    public class Pong : Packet
    {
        [Field]
        public long Payload { get; set; }

        public Pong(long payload)
        {
            Payload = payload;
        }
    }
}