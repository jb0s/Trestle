using System;
using Trestle.Enums;

namespace Trestle.Attributes
{
    /// <summary>
    /// Assign this to a Packet class to mark it as a ClientBound (server to client) packet.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ClientBoundAttribute : Attribute
    {
        public byte Id { get; set; }
        
        public ClientBoundAttribute(StatusPacket packet)
        {
            Id = (byte) packet;
        }
        
        public ClientBoundAttribute(LoginPacket packet)
        {
            Id = (byte) packet;
        }
        
        public ClientBoundAttribute(PlayPacket packet)
        {
            Id = (byte) packet;
        }
    }
}