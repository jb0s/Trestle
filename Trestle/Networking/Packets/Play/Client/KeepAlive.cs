using Trestle.Enums;
using Trestle.Attributes;
using Trestle.Enums.Packets.Client;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.KeepAlive)]
    public class KeepAlive : Packet
    {
        [Field]
        [VarInt]
        public long KeepAliveId { get; set; }

        public KeepAlive(int keepAliveId)
        {
            KeepAliveId = keepAliveId;
        }
    }
}