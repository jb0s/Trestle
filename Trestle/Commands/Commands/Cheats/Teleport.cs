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
        [Alias("tp")]
        public void TeleportCommand(double x, double y, double z)
            => Player.Teleport(new Vector3(x, y, z));
    }
}