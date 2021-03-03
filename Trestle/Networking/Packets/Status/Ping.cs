using Trestle.Attributes;
using Trestle.Enums;

namespace Trestle.Networking.Packets.Status
{
    [ServerBound(StatusPacket.Ping)]
    public class Ping : Packet
    {
        [Field]
        public long Payload { get; set; }

        public override void HandlePacket()
            => Client.SendPacket(new Pong(Payload));
    }
}