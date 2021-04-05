using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Enums.Packets.Client;

namespace Trestle.Networking.Packets.Login.Client
{
    [ClientBound(LoginPacket.SetCompression)]
    public class SetCompression : Packet
    {
        [Field]
        [VarInt]
        public int Threshold { get; set; } = -1;

        public SetCompression(int threshold)
        {
            Threshold = threshold;
        }
    }
}