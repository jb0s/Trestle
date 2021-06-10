using System;
using Trestle.Enums;

namespace Trestle.Levels.Items.Attributes
{
    public class ItemAttribute : Attribute
    {
        public Material Material { get; set; }

        public ItemAttribute(Material material)
        {
            Material = material;
        }
    }
}