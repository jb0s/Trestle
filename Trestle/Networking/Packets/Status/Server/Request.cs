using System;
using System.Threading.Tasks;
using Trestle.Configuration.Service;
using Trestle.Networking.Attributes;
using Trestle.Networking.Enums.Server;
using Trestle.Networking.Packets.Status.Client;

namespace Trestle.Networking.Packets.Status.Server
{
    [ServerBound(StatusPacket.Request)]
    public class Request : Packet
    {
        public override async Task Handle()
        {
            Client.SendPacket(new Response(ConfigService));
        }
    }
}