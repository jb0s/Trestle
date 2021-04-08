using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Utils;

namespace Trestle.Commands.Commands.Cheats
{
    public class GameMode : Command
    {
        [Command("gamemode")]
        [Description("Sets the game mode.")]
        [Alias("gm")]
        public void SetGameMode(int gameMode)
        {
            if (gameMode > 3)
            {
                Client.Player.SendChat(new MessageComponent($"{ChatColor.Red}Invalid usage! {ChatColor.Reset}/gamemode <0-3>"));
                return;
            }

            var mode = (Enums.GameMode)gameMode;
            Client.Player.SetGamemode(mode, false);
        }
        
        [Command("gmc")]
        [Description("Sets the game mode to creative mode.")]
        public void CreativeMode()
            => Client.Player.SetGamemode(Enums.GameMode.Creative, false);
        
        [Command("gms")]
        [Description("Sets the game mode to survival mode.")]
        public void SurvivalMode()
            => Client.Player.SetGamemode(Enums.GameMode.Survival, false);
        
        [Command("gma")]
        [Description("Sets the game mode to adventure mode.")]
        public void AdventureMode()
            => Client.Player.SetGamemode(Enums.GameMode.Adventure, false);
        
        [Command("gmsp")]
        [Description("Sets the game mode to spectator mode.")]
        public void SpectatoreMode()
            => Client.Player.SetGamemode(Enums.GameMode.Spectator, false);
    }
}