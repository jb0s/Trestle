using System;

namespace Trestle.Attributes
{
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