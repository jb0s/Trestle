using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Login.Client
{
    [ClientBound(LoginPacket.Disconnect)]
    public class Disconnect : Packet
    {
        [Field]
        public MessageComponent Reason { get; set; }

        public Disconnect(string reason)
        {
            Reason = new MessageComponent(reason);
        }
        
        public Disconnect(MessageComponent reason)
        {
            Reason = reason;
        }
    }
}