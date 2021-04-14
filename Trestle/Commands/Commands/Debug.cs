using Trestle.Attributes;
using Trestle.Networking.Packets.Play.Client;
using Trestle.Utils;

namespace Trestle.Commands.Commands
{
    public class Debug : Command
    {
        [Command("pain")]
        [Description("pain.")]
        public void DebugPain(int amount)
        {
            Player.HealthManager.Pain(amount);
        }
    }
}