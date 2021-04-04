using System;

namespace Trestle.Attributes
{
    /// <summary>
    /// Assign this to a property in a Packet class to mark it as a property that should be (de)serialized.
    /// </summary>
    public class FieldAttribute : Attribute
    {
        public Type OverrideType { get; set; }
        
        public FieldAttribute()
        {
        }
        
        public FieldAttribute(Type type)
        {
            OverrideType = type;
        }
    }
}