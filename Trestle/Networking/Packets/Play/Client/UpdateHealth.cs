using Trestle.Attributes;
using Trestle.Entity;
using Trestle.Enums.Packets.Client;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.UpdateHealth)]
    public class UpdateHealth : Packet
    {
        [Field]
        public float Health { get; set; }
        
        [Field]
        [VarInt]
        public int Food { get; set; }
        
        [Field]
        public float FoodSaturation { get; set; }

        public UpdateHealth(Player player)
        {
            Health = player.HealthManager.Health;
            Food = 20;
            FoodSaturation = 0;
        }
    }
}