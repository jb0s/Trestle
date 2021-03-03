using System;
using Trestle.Enums;

namespace Trestle.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ServerBoundAttribute : Attribute
    {
        public byte Id { get; set; }
        
        public ClientState State { get; set; }
        
        public ServerBoundAttribute(HandshakingPacket packet)
        {
            Id = (byte) packet;
            State = ClientState.Handshaking;
        }
        
        public ServerBoundAttribute(StatusPacket packet)
        {
            Id = (byte) packet;
            State = ClientState.Status;
        }
        
        public ServerBoundAttribute(LoginPacket packet)
        {
            Id = (byte) packet;
            State = ClientState.Login;
        }
        
        public ServerBoundAttribute(PlayPacket packet)
        {
            Id = (byte) packet;
            State = ClientState.Play;
        }
    }
}