using System;
using System.Text.Json;
using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.Client_ChatMessage)]
    public class ChatMessage : Packet
    {
        [Field]
        public string Message { get; set; }

        [Field]
        public byte Position { get; set; } = 0;
        
        public ChatMessage(MessageComponent messageComponent)
        {
            Message = JsonSerializer.Serialize(messageComponent);
        }
    }
}