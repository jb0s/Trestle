using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Worlds;

namespace Trestle.Networking.Packets.Play
{
    [ClientBound(PlayPacket.ChunkData)]
    public class ChunkData : Packet
    {
        [Field] 
        public byte[] Chunk { get; set; } 
        
        public bool Unloader = false;

        public ChunkData(Chunk chunk)
        {
            Chunk = chunk.GetBytes(Unloader);
        }
    }
}