using System;
using Trestle.Networking.Enums;
using Trestle.Networking.Enums.Server;

namespace Trestle.Networking.Attributes
{
    public class ServerBoundAttribute : Attribute
    {
        public byte Id { get; set; }
        
        public State State { get; set; }
        
        public ServerBoundAttribute(HandshakingPacket packet)
        {
            Id = (byte) packet;
            State = State.Handshaking;
        }
        
        public ServerBoundAttribute(StatusPacket packet)
        {
            Id = (byte) packet;
            State = State.Status;
        }
        
        public ServerBoundAttribute(LoginPacket packet)
        {
            Id = (byte) packet;
            State = State.Login;
        }
    }
}