using Trestle.Attributes;
using Trestle.Enums.Packets.Client;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.PlayerListHeaderAndFooter)]
    public class PlayerListHeaderAndFooter : Packet
    {
        [Field]
        public MessageComponent Header { get; set; }
        
        [Field]
        public MessageComponent Footer { get; set; }

        public PlayerListHeaderAndFooter(MessageComponent header, MessageComponent footer)
        {
            Header = header;
            Footer = footer;
        }
    }
}