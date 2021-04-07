using System;
using System.Text;
using Trestle.Attributes;
using Trestle.Enums.Packets.Server;

namespace Trestle.Networking.Packets.Play.Server
{
    [ServerBound(PlayPacket.PluginMessage)]
    public class PluginMessage : Packet
    {
        [Field]
        public string Channel { get; set; }
        
        [Field]
        public byte[] Data { get; set; }

        public override void HandlePacket()
        {

        }
    }
}