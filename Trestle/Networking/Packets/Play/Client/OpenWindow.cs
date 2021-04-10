using System;
using System.Reflection;
using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Enums.Packets.Client;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.OpenWindow)]
    public class OpenWindow : Packet
    {
        [Field]
        [VarInt]
        public int WindowId { get; set; } 
        
        [Field]
        public string WindowType { get; set; }
        
        [Field]
        public MessageComponent WindowTitle { get; set; }

        [Field] 
        public byte Slots { get; set; }
        
        public OpenWindow(int id, WindowType type, MessageComponent title, byte slots = 0)
        {
            WindowId = id;
            WindowType = type.GetType().GetMember(type.ToString())[0].GetCustomAttribute<DescriptionAttribute>(false)?.Description;
            WindowTitle = title;
            Slots = slots;
        }
    }
}