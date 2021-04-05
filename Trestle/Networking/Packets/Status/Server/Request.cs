using System;
using System.Text.Json;
using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Enums.Packets.Server;
using Trestle.Networking.Packets.Status.Client;

namespace Trestle.Networking.Packets.Status.Server
{
    [ServerBound(StatusPacket.Request)]
    public class Request : Packet
    {
        public override void HandlePacket()
            => Client.SendPacket(new Response());
    }
}