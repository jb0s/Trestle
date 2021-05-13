using System;
using Trestle.Networking.Attributes;
using Trestle.Networking.Enums.Server;

namespace Trestle.Networking.Packets.Login.Server
{
    [ServerBound(LoginPacket.LoginStart)]
    public class LoginStart : Packet
    {
        [Field]
        public string Name { get; set; }

        public override void Handle()
        {
        }
    }
}