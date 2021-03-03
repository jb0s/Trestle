using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Play
{
    [ClientBound(PlayPacket.Client_PlayerPositionAndLook)]
    public class PlayerPositionAndLook : Packet
    {
        [Field] 
        public double X { get; set; } = Globals.WorldManager.MainWorld.GetSpawnPoint().X;
        
        [Field]
        public double Y { get; set; } = Globals.WorldManager.MainWorld.GetSpawnPoint().Y;
        
        [Field]
        public double Z { get; set; } = Globals.WorldManager.MainWorld.GetSpawnPoint().Z;

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
            TeleportId = client.TeleportTicket;
        }
    }
}