using Trestle.Enums;
using Trestle.Levels.Items.Attributes;

namespace Trestle.Levels.Items
{
    [Item(Material.DiamondSword)]
    public class DiamondSword : IMeleeItem
    {
        public int MaxStackSize => 1;
        public int MaxDurability => 1561;
    }
}