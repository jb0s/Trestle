﻿using Trestle.Attributes;
using Trestle.Enums;

namespace Trestle.Networking.Packets.Login
{
    [ClientBound(LoginPacket.SetCompression)]
    public class SetCompression : Packet
    {
        [Field]
        [VarInt]
        public int Threshold { get; set; } = -1;

        public SetCompression(int threshold)
        {
            Threshold = threshold;
        }
    }
}