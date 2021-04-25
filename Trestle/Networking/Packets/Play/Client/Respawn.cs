using Trestle.Attributes;
using Trestle.Enums.Packets.Client;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.Respawn)]
    public class Respawn : Packet
    {
        [Field]
        public int Dimension => 0;

        [Field]
        public byte Difficulty => 2;

        [Field]
        public byte GameMode => (byte)Client.Player.GameMode;

        [Field]
        public string LevelType => "flat";
    }
}