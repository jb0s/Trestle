using System;
using Trestle.Levels.Enums;

namespace Trestle.Levels.Attributes
{
    public class DimensionAttribute : Attribute
    {
        public DimensionType DimensionType { get; set; }

        public DimensionAttribute(DimensionType dimensionType)
        {
            DimensionType = dimensionType;
        }
    }
}