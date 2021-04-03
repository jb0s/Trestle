using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Play.Client
{
    [ClientBound(PlayPacket.Client_SoundEffect)]
    public class SoundEffect : Packet
    {
        [Field]
        public string SoundName { get; set; }
        
        [Field]
        public int EffectPositionX { get; set; }
        
        [Field]
        public int EffectPositionY { get; set; }
        
        [Field]
        public int EffectPositionZ { get; set; }
        
        [Field]
        public float Volume { get; set; }
        
        [Field]
        public byte Pitch { get; set; }

        public SoundEffect(string name, Vector3 position, float volume = 1, byte pitch = 63)
        {
            SoundName = name;

            EffectPositionX = (int)position.X * 8;
            EffectPositionY = (int)position.Y * 8;
            EffectPositionZ = (int)position.Z * 8;

            Volume = volume;
            Pitch = pitch;
        }
    }
}