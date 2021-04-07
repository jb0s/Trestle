using Trestle.Attributes;
using Trestle.Enums.Packets.Server;

namespace Trestle.Networking.Packets.Play.Server
{
    [ServerBound(PlayPacket.TeleportConfirm)]
    public class TeleportConfirm : Packet
    {
        [Field]
        [VarInt]
        public int TeleportId { get; set; }
        
        public override void HandlePacket()
        {
            // idk
        }
    }
}