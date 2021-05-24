using System.Threading.Tasks;
using Trestle.Entities.Players;
using Trestle.Enums;
using Trestle.Levels.Enums;

namespace Trestle.Levels.Items
{
    public interface IItem
    {
        /// <summary>
        /// The maximum amount of this item that can be held in a inventory slot.
        /// </summary>
        public int MaxStackSize => 64;

        /// <summary>
        /// How much durability this item has.
        /// Decreases by 1 upon item usage.
        /// Set to 0 to make it unbreakable.
        /// </summary>
        public int MaxDurability => 0;

        /// <summary>
        /// Can this item withstand lava and fire?
        /// </summary>
        public bool IsFireResistant => false;

        /// <summary>
        /// Can this item be enchanted?
        /// </summary>
        public bool IsEnchantable => MaxStackSize == 1 && MaxDurability > 0;

        public async Task<InteractionResult> Use(Player player, PlayerHand hand)
            => InteractionResult.Pass;
    }
}