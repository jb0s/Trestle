using System;
using System.Text.Json;
using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.Client_Disconnect)]
    public class Disconnect : Packet
    {
        [Field] 
        public string Reason { get; set; }

        public Disconnect(MessageComponent reason)
        {
            Reason = JsonSerializer.Serialize(reason);
        }
    }
}