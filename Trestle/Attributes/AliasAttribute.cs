using System;

namespace Trestle.Attributes
{
    public class AliasAttribute : Attribute
    {
        public string[] Aliases { get; set; }

        public AliasAttribute(params string[] aliases)
        {
            Aliases = aliases;
        }
    }
}