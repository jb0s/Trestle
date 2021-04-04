using System;
using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Play.Server
{
    [ServerBound(PlayPacket.Server_PlayerPositionAndLook)]
    public class PlayerPositionAndLook : Packet
    {
        [Field]
        public double X { get; set; }
        
        [Field]
        public double Y { get; set; }
        
        [Field]
        public double Z { get; set; }
        
        [Field]
        public float Yaw { get; set; }
        
        [Field]
        public float Pitch { get; set; }
        
        [Field]
        public bool OnGround { get; set; }

        public override void HandlePacket()
        {
            Entity.Player player = Client.Player;

            Vector3 newLocation = new Vector3(X, Y, Z);
            player.PositionChanged(newLocation, Yaw, Pitch, OnGround);
        }
    }
}