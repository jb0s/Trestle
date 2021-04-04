using System;

namespace Trestle.Attributes
{
    /// <summary>
    /// Assign this to a Packet class to make it not disconnect a client when the packet (de)serialization fails.
    /// Mainly used for packets that change its layout when it's not supposed to do anything.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class IgnoreExceptionsAttribute : Attribute
    {
    }
}