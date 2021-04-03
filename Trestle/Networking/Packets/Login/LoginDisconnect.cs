using System;
using System.Text.Json;
using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Play.Client
{
    // Shares ID with Disconnect
    [ClientBound(LoginPacket.LoginStart)]
    public class LoginDisconnect : Packet
    {
        [Field] 
        public string Reason { get; set; }

        public LoginDisconnect(MessageComponent reason)
        {
            Reason = JsonSerializer.Serialize(reason);
        }
    }
}