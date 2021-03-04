using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Play
{
    [ClientBound(PlayPacket.Client_PlayerPositionAndLook)]
    public class PlayerPositionAndLook : Packet
    {
        [Field] 
        public double X { get; set; } = 0;
        
        [Field]
        public double Y { get; set; } = 5;
        
        [Field]
        public double Z { get; set; } = 0;

        [Field] 
        public float Yaw { get; set; } = 0f;

        [Field] 
        public float Pitch { get; set; } = 0f;

        [Field]
        public byte Flags { get; set; } = 111;
        
        [Field]
        [VarInt]
        public int TeleportId { get; set; }
        
        public PlayerPositionAndLook(Networking.Client client, Location location)
        {
            X = location.X;
            Y = location.Y;
            Z = location.Z;
            Yaw = location.Yaw;
            Pitch = location.Pitch;
            TeleportId = 0; // TODO: Teleport confirming
        }
    }
}