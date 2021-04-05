using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Enums.Packets.Client;

namespace Trestle.Networking.Packets.Status.Client
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