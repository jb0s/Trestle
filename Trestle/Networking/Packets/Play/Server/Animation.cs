using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Enums.Packets.Server;

namespace Trestle.Networking.Packets.Play.Server
{
    [ServerBound(PlayPacket.Animation)]
    public class Animation : Packet
    {
        [Field]
        [VarInt]
        public int Hand { get; set; }

        public override void HandlePacket()
        {
        }
    }
}