using Trestle.Networking.Attributes;
using Trestle.Networking.Enums.Client;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.ChunkData)]
    public class ChunkData : Packet
    {


        public ChunkData()
        {
        }
    }
}