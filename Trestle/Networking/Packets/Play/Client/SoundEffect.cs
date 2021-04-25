using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Enums.Packets.Client;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.SoundEffect)]
    public class SoundEffect : Packet
    {
        [Field]
        [VarInt]
        public int SoundId { get; set; }
        
        [Field(typeof(int))]
        [VarInt]
        public SoundCategory SoundCategory { get; set; }
        
        [Field]
        public int EffectPositionX { get; set; }
        
        [Field]
        public int EffectPositionY { get; set; }
        
        [Field]
        public int EffectPositionZ { get; set; }
        
        [Field]
        public float Volume { get; set; }
        
        [Field]
        public float Pitch { get; set; }
        
        public SoundEffect(int soundId, SoundCategory category, Vector3 position, float volume = 1.0f, float pitch = 1.0f)
        {
            SoundId = soundId;
            SoundCategory = category;
            
            EffectPositionX = (int)position.X * 8;
            EffectPositionY = (int)position.Y * 8;
            EffectPositionZ = (int)position.Z * 8;

            Volume = volume;
            Pitch = pitch;
        }
    }
}