using Trestle.Networking.Attributes;
using Trestle.Networking.Enums.Client;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.KeepAlive)]
    public class KeepAlive : Packet
    {
        [Field]
        public long KeepAliveId { get; set; }

        public KeepAlive(long keepAliveId)
        {
            KeepAliveId = keepAliveId;
        }
    }
}