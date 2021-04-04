using System;
using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.Client_PlayerPositionAndLook)]
    public class PlayerPositionAndLook : Packet
    {
        [Field] 
        public double X { get; set; } = 0;
        
        [Field]
        public double Y { get; set; } = 0;
        
        [Field]
        public double Z { get; set; } = 0;

        [Field] 
        public float Yaw { get; set; } = 0f;

        [Field] 
        public float Pitch { get; set; } = 0f;

        [Field]
        public byte Flags { get; set; } = 0x00;
        
        public PlayerPositionAndLook(Location location)
        {
            X = location.X;
            Y = location.Y;
            Z = location.Z;
            Yaw = location.Yaw;
            Pitch = location.Pitch;
        }
    }
}