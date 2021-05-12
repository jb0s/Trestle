using System;

namespace Trestle.Networking.Attributes
{
    public class FieldAttribute : Attribute
    {
        public Type OverrideType { get; set; }
        
        public FieldAttribute() { }
        
        public FieldAttribute(Type type)
            => OverrideType = type;
    }
}