using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Enums.Packets.Client;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.BlockChange)]
    public class BlockChange : Packet
    {
        [Field]
        public Vector3 Location { get; set; }
        
        [Field]
        [VarInt]
        public int BlockId { get; set; }

        public BlockChange(Vector3 location, Material block, int meta)
        {
            Location = location;
            BlockId = (int)block << 4 | meta & 15;
        }
    }
}