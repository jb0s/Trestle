using Trestle.Attributes;
using Trestle.Entity;
using Trestle.Networking.Packets.Play.Client;
using Trestle.Utils;

namespace Trestle.Commands.Commands.Cheats
{
    public class Teleport : Command
    {
        [Command("teleport")]
        [Description("Teleports a player to a location.")]
        public void TeleportCommand(double x, double y, double z)
        {
            Player.Location = new Location(x, y, z);
            Client.SendPacket(new PlayerPositionAndLook(Player.Location));
            Player.ForceChunkReload = true;
        }
    }
}