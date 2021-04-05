using System;
using Trestle.Attributes;
using Trestle.Blocks;
using Trestle.Enums;
using Trestle.Enums.Packets.Server;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Play.Server
{
    [ServerBound(PlayPacket.ChatMessage)]
    public class ChatMessage : Packet
    {
        [Field]
        public string Message { get; set; }

        public override void HandlePacket()
        {
            if (Message.StartsWith("/"))
            {
                Client.Player.Inventory.AddItem(new ItemStack(new Block(Material.Cactus), 20));
                return;
            }
            
            Globals.BroadcastChat($"<{Client.Player.Username}> {Message}");
            Logger.Info($"<{Client.Player.Username}> {Message}");
        }
    }
}