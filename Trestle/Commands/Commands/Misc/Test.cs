using System;
using System.Reflection;
using Trestle.Attributes;
using Trestle.Enums;

namespace Trestle.Commands.Commands.Misc
{
    public class Test : Command
    {
        [Command("test")]
        [Description("a test command")]
        [Alias("test1", "test2")]
        public void Testing()
        {
        }
    }
}