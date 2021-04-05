using System;
using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Enums.Packets.Client;

namespace Trestle.Networking.Packets.Login.Client
{
    [ClientBound(LoginPacket.LoginSuccess)]
    public class LoginSuccess : Packet
    {
        [Field]
        public string Uuid { get; set; }
        
        [Field]
        public string Username { get; set; }

        public LoginSuccess(string uuid, string username)
        {
            Uuid = uuid;
            Username = username;
        }
    }
}