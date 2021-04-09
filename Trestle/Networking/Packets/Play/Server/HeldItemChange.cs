using Trestle.Attributes;
using Trestle.Enums.Packets.Server;

namespace Trestle.Networking.Packets.Play.Server
{
    [ServerBound(PlayPacket.HeldItemChange)]
    public class HeldItemChange : Packet
    {
        [Field]
        public short Slot { get; set; }

        public override void HandlePacket()
        {
            if(Slot > 8 || Slot < 0)
                Client.Player.KickAntiCheat("Illegal item slot");
            
            Client.Player.Inventory.CurrentSlot = Slot;
        }
    }
}