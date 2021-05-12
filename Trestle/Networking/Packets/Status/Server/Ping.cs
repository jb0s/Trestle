using Trestle.Networking.Attributes;
using Trestle.Networking.Enums.Server;
using Trestle.Networking.Packets.Status.Client;

namespace Trestle.Networking.Packets.Status.Server
{
    [ServerBound(StatusPacket.Ping)]
    public class Ping : Packet
    {
        [Field]
        public long Payload { get; set; }

        public override void Handle()
            => Client.SendPacket(new Pong());
    }
}