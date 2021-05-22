using System;
using Trestle.Networking.Enums;
using Trestle.Networking.Enums.Client;

namespace Trestle.Networking.Attributes
{
    public class ClientBoundAttribute : Attribute
    {
        public byte Id { get; set; }
        
        public State State { get; set; }
        
        public ClientBoundAttribute(StatusPacket packet)
        {
            Id = (byte) packet;
            State = State.Status;
        }
        
        public ClientBoundAttribute(LoginPacket packet)
        {
            Id = (byte) packet;
            State = State.Login;
        }
        
        public ClientBoundAttribute(PlayPacket packet)
        {
            Id = (byte) packet;
            State = State.Play;
        }
    }
}