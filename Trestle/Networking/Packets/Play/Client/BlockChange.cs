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
        public long Location { get; set; }
        
        [Field]
        [VarInt]
        public int BlockId { get; set; }

        public BlockChange(Vector3 location, Material block)
        {
            Location = (((long)location.X & 0x3FFFFFF) << 38) | (((long)location.Z & 0x3FFFFFF) << 12) | ((long)location.Y & 0xFFF);
            BlockId = (int) block;
        }
    }
}