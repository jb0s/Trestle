using System;
using System.Threading.Tasks;
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

        public override async Task Handle()
            => Client.SendPacket(new Pong(Payload));
    }
}