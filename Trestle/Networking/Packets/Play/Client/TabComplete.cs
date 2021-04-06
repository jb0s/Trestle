using System.Collections.Generic;
using Trestle.Attributes;
using Trestle.Enums.Packets.Client;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.TabComplete)]
    public class TabComplete : Packet
    {
        [Field]
        public string[] Matches { get; set; }

        public TabComplete(string text)
        {
            var matches = new List<string>();
            foreach (var command in Globals.CommandManager.Commands.Keys)
            {
                if (command.StartsWith(text.Substring(1)))
                {
                    matches.Add("/" + command);
                }
            }

            Matches = matches.ToArray();
        }
    }
}