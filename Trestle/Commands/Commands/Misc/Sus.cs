using Trestle.Attributes;
using Trestle.Enums;

namespace Trestle.Commands.Commands.Misc
{
    public class Sus : Command
    {
        [Command("sus")]
        [Description("ඞ")]
        public void SusCommand()
        {
            Player.SendChat($"{ChatColor.Red}amogus");
            Player.SendChat(Player.Location.Yaw.ToString());
        }
    }
}