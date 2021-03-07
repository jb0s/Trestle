using System;
using Trestle.Attributes;
using Trestle.Enums;

namespace Trestle.Networking.Packets.Login
{
    [ClientBound(LoginPacket.LoginSuccess)]
    public class LoginSuccess : Packet
    {
        [Field]
        public byte[] Uuid { get; set; }
        
        [Field]
        public string Username { get; set; }

        public LoginSuccess(string username, Guid uuid)
        {
            Username = username;
            Uuid = uuid.ToByteArray();
            
            Logger.Info($"Player {username} with UUID {uuid} logged in successfully");
        }
    }
}