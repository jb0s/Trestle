using System;
using Trestle.Attributes;
using Trestle.Enums;

namespace Trestle.Networking.Packets.Play.Server
{
    [ServerBound(PlayPacket.Server_ChatMessage)]
    public class ChatMessage : Packet
    {
        [Field]
        public string Message { get; set; }

        public override void HandlePacket()
        {
            if (Message.StartsWith("/"))
            {
                return;
            }
            
            Globals.BroadcastChat($"<{Client.Player.Username}> §5{Message}");
            Logger.Info($"<{Client.Player.Username}> {Message}");
        }
    }
}