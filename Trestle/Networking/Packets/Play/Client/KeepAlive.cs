using Trestle.Enums;
using Trestle.Attributes;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.Client_KeepAlive)]
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