using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Trestle.Attributes;
using Trestle.Enums;

namespace Trestle.Commands.Commands.Help
{
    public class Help : Command
    {
        [Command("help")]
        [Description("Shows a list of all commands.")]
        [Alias("h")]
        public void HelpCommand()
        {
            var commands = new List<string>();
            foreach (var (command, (type, method)) in Globals.CommandManager.Commands)
            {
                var aliases = method.GetCustomAttribute<AliasAttribute>();
                if (aliases != null && aliases.Aliases.Any(x => x == command))
                    continue;
                    
                var description = method.GetCustomAttribute<DescriptionAttribute>();

                commands.Add($"{ChatColor.DarkGray}/{ChatColor.Gray}{command} {ChatColor.DarkGray}-{ChatColor.Gray} {description?.Description ?? "It's a mystery..."}");
            }
            
            Player.SendChat($"{ChatColor.Aqua}Trestle {ChatColor.DarkGray}- {ChatColor.Gray}Commands {ChatColor.DarkGray}({ChatColor.Gray}1/{(commands.Count - 1) / 8 + 1}{ChatColor.DarkGray})\n" +
                            $"{ChatColor.DarkGray}{ChatColor.StrikeThrough}-----------------\n" +
                            string.Join('\n', commands));
        }
    }
}