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
        public byte MessageType { get; set; } = (byte)ChatMessageType.ChatBox;
        
        [Field]
        public Guid Player { get; set; } 
        
        public ChatMessage(MessageComponent messageComponent)
        {
            Message = JsonSerializer.Serialize(messageComponent);
            Player = Guid.NewGuid();
        }
    }
}