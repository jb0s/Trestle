using System;
using Trestle.Networking.Attributes;
using Trestle.Networking.Enums.Server;
using Trestle.Networking.Packets.Status.Client;

namespace Trestle.Networking.Packets.Status.Server
{
    [ServerBound(StatusPacket.Request)]
    public class Request : Packet
    {
        public override void Handle()
            => Client.SendPacket(new Response());
    }
}