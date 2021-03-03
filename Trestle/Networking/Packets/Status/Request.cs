using System;
using Trestle.Attributes;
using Trestle.Enums;

namespace Trestle.Networking.Packets.Status
{
    [ServerBound(StatusPacket.Request)]
    public class Request : Packet
    {
        public override void HandlePacket()
        {
            Console.WriteLine("very special!");
        }
    }
}