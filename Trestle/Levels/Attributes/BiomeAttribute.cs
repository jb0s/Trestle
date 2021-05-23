using System;
using Trestle.Levels.Enums;

namespace Trestle.Levels.Attributes
{
    public class BiomeAttribute : Attribute
    {
        public BiomeType BiomeType { get; set; }

        public BiomeAttribute(BiomeType biomeType)
        {
            BiomeType = biomeType;
        }
    }
}