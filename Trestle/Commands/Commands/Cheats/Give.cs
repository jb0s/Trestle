using System;
using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Utils;

namespace Trestle.Commands.Commands.Cheats
{
    public class Give : Command
    {
        [Command("give")]
        [Description("Gives the player an item.")]
        public void GiveItem(string item, int count)
        {
            var material = Enum.Parse<Material>(item);
            Player.Inventory.AddItem((short)material, count, 0);
            
            Player.SendChat($"Gave {Player.Username} {count} {material.ToString()}");
        }
    }
}