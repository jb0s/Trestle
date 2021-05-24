using System;
using Trestle.Enums;

namespace Trestle.Levels.Items
{
    public class ItemStack
    {
        public IItem Item;
        public int Count;

        public ItemStack(Material material, int count)
        {
            throw new NotImplementedException();
        }
    }
}