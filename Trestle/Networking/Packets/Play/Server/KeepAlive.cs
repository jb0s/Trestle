using System;
using System.Threading.Tasks;
using Trestle.Networking.Attributes;
using Trestle.Networking.Enums.Server;

namespace Trestle.Networking.Packets.Play.Server
{
    [ServerBound(PlayPacket.KeepAlive)]
    public class KeepAlive : Packet
    {
        [Field]
        public long KeepAliveId { get; set; }

        public override Task Handle()
        {
            Client.MissedKeepAlives = 0;
            Client.LastKeepAliveSuccess = DateTime.Now.Millisecond;
            
            return Task.CompletedTask;
        }
    }
}