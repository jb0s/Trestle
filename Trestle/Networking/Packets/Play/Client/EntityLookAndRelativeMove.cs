using System;
using Trestle.Attributes;
using Trestle.Enums.Packets.Client;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.EntityLookAndRelativeMove)]
    public class EntityLookAndRelativeMove : Packet
    {
        [Field]
        [VarInt]
        public int EntityId { get; set; }
        
        [Field]
        public short DeltaX { get; set; }
        
        [Field]
        public short DeltaY { get; set; }
        
        [Field]
        public short DeltaZ { get; set; }
        
        [Field]
        public byte Yaw { get; set; }
        
        [Field]
        public byte Pitch { get; set; }
        
        [Field]
        public bool OnGround { get; set; }
        
        public EntityLookAndRelativeMove(int entityId, Location prevLocation, Location newLocation)
        {
            EntityId = entityId;

            DeltaX = (short)((newLocation.X * 32 - prevLocation.X * 32) * 128);
            DeltaY = (short)((newLocation.Y * 32 - prevLocation.Y * 32) * 128);
            DeltaZ = (short)((newLocation.Z * 32 - prevLocation.Z * 32) * 128);
            
            byte newYaw = (byte)Location.GetBodyRotation(newLocation.Yaw * 256 / 360);
            byte newPitch = (byte)Location.GetBodyRotation(newLocation.Pitch * 256 / 360);
            bool doTheMaths = Math.Abs(newYaw - newLocation.Yaw) >= 1 || Math.Abs(newPitch - newLocation.Pitch) >= 1;

            if (doTheMaths)
            {
                Yaw = newYaw;
                Pitch = newPitch;
            }
            else
            {
                Yaw = (byte)newLocation.Yaw;
                Pitch = (byte)newLocation.Pitch;
            }

            OnGround = newLocation.OnGround;
        }
    }
}