using System;

namespace Trestle.Attributes
{
    public class IndexAttribute : Attribute
    {
        public int Index { get; set; }
        
        public IndexAttribute(int index)
        {
            Index = index;
        }
    }
}