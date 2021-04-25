using System;
using System.IO;
using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Enums.Packets.Client;
using Trestle.Utils;
using Trestle.Worlds;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.ChunkData)]
    public class ChunkData : Packet
    {
        [Field]
        public byte[] Data { get; set; }

        public ChunkData(byte[] chunk)
        {
            Data = chunk;
        }
    }
}