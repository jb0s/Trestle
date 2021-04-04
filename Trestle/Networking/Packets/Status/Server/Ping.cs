using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Networking.Packets.Status.Client;

namespace Trestle.Networking.Packets.Status.Server
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