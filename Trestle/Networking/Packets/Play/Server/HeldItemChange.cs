using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Enums.Packets.Server;
using Trestle.Networking.Packets.Play.Client;

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
            
            Player.Inventory.CurrentSlot = Slot;
            World.BroadcastPacket(new EntityEquipment(Player.EntityId, EntityEquipmentSlot.MainHand, Player.Inventory.CurrentItem), Player);
        }
    }
}