using System;
using System.Text.Json;
using Trestle.Attributes;
using Trestle.Enums;

namespace Trestle.Networking.Packets.Status
{
    [ServerBound(StatusPacket.Request)]
    public class Request : Packet
    {
        public override void HandlePacket()
            => Client.SendPacket(new Response());
    }
}