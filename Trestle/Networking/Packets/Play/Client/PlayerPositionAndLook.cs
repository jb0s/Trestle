using Trestle.Entities.Players;
using Trestle.Networking.Attributes;
using Trestle.Networking.Enums.Client;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.PlayerPositionAndLook)]
    public class PlayerPositionAndLook : Packet
    {
        [Field]
        public Location Location { get; set; }

        [Field]
        public byte Flags { get; set; } = 0x00;

        [Field] 
        [VarInt] 
        public int TeleportId { get; set; } = 0;

        public PlayerPositionAndLook(Player player)
        {
            Location = player.Location;
        }
    }
}