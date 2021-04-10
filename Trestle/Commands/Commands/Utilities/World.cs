using Trestle.Attributes;
using Trestle.Enums;

namespace Trestle.Commands.Commands.Utilities
{
    public class World : Command
    {
        [Command("save")]
        [Description("Saves the world.")]
        public void Save()
        {
            Client.Player.SendChat($"{ChatColor.Gray}Saving the world...");
            
            Player.World.Save();
            
            Client.Player.SendChat($"{ChatColor.Green}Saved the world");
        }
    }
}