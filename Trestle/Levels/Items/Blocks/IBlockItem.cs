using Trestle.Enums;

namespace Trestle.Levels.Items.Blocks
{
    public interface IBlockItem : IItem
    {
        /// <summary>
        /// The type of item required to drop this block's loot.
        /// </summary>
        public ItemType RequiredTool => ItemType.All;

        /// <summary>
        /// The items that this block drops when broken by the right tool.
        /// </summary>
        /// <returns></returns>
        public Material[] Loot => new Material[] { Material.Stone };
    }
}