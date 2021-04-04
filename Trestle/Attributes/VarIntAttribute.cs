using System;

namespace Trestle.Attributes
{
    /// <summary>
    /// Assign this to a property in a Packet class to mark it as a VarInt property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class VarIntAttribute : Attribute
    {
    }
}