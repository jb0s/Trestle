using Trestle.Enums;
using Trestle.Attributes;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.Client_KeepAlive)]
    public class KeepAlive : Packet
    {
        [Field]
        [VarInt]
        public int KeepAliveId { get; set; }

        public KeepAlive(int keepAliveId)
        {
            KeepAliveId = keepAliveId;
        }
    }
}