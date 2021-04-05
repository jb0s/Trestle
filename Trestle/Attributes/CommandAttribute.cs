using System;

namespace Trestle.Attributes
{
    public class CommandAttribute : Attribute
    {
        public string Command { get; set; }
        
        public CommandAttribute(string command)
        {
            Command = command;
        }
    }
}