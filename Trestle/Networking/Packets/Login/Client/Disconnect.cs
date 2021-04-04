using System.Text.Json;
using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Login.Client
{
    [ClientBound(LoginPacket.Disconnect)]
    public class Disconnect : Packet
    {
        [Field]
        public string Reason { get; set; }

        public Disconnect(string reason)
        {
            Reason = JsonSerializer.Serialize(new MessageComponent(reason));
        }
        
        public Disconnect(MessageComponent reason)
        {
            Reason = JsonSerializer.Serialize(reason);
        }
    }
}