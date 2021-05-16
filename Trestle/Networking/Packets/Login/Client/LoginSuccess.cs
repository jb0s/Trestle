using System;
using Trestle.Networking.Attributes;
using Trestle.Networking.Enums.Client;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Login.Client
{
    [ClientBound(LoginPacket.LoginSuccess)]
    public class LoginSuccess : Packet
    {
        [Field]
        public string Uuid { get; set; }
        
        [Field]
        public string Username { get; set; }

        public LoginSuccess(Uuid uuid, string username)
        {
            Uuid = uuid.ToString();
            Username = username;
        }
    }
}