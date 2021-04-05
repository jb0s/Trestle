using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Enums.Packets.Client;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.NamedSoundEffect)]
    public class NamedSoundEffect : Packet
    {
        [Field]
        public string SoundName { get; set; }
        
        [Field]
        [VarInt]
        public int SoundCategory { get; set; }
        
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

        public NamedSoundEffect(string name, SoundCategory category, Vector3 position, float volume = 1.0f, float pitch = 1.0f)
        {
            SoundName = name;
            SoundCategory = (int)category;
            
            EffectPositionX = (int)position.X * 8;
            EffectPositionY = (int)position.Y * 8;
            EffectPositionZ = (int)position.Z * 8;

            Volume = volume;
            Pitch = pitch;
        }
    }
}