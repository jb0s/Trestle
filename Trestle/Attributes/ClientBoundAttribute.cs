using System;
using Trestle.Enums;

namespace Trestle.Attributes
{
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