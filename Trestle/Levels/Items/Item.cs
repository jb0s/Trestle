using Trestle.Enums;

namespace Trestle.Levels.Items
{
    public class Item
    {
        public virtual Material Material { get; protected set; } = Material.Air;
        public virtual int MaxStackSize { get; protected set; } = 64;
        public virtual int MaxDurability { get; protected set; } = 0;
        public virtual bool IsFireResistant { get; protected set; } = false;

        
        
        /// <summary>
        /// Can this item break with low durability?
        /// </summary>
        /// <returns></returns>
        public bool CanBeDepleted()
            => MaxDurability > 0;
    }
}