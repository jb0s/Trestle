using System;
using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Play.Server
{
    [ServerBound(PlayPacket.Server_PlayerLook)]
    public class PlayerLook : Packet
    {
        [Field] 
        public float Yaw { get; set; }
        
        [Field]
        public float Pitch { get; set; }
        
        [Field]
        public bool OnGround { get; set; }

        public override void HandlePacket()
        {
            Entity.Player player = Client.Player;
            player.PositionChanged(player.Location.ToVector3(), Yaw, Pitch, OnGround);
        }
    }
}