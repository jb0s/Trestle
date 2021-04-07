using Trestle.Attributes;

namespace Trestle.Entity
{
    public class LivingMetadata : Metadata
    {
        [Field] 
        [Index(6)] 
        public byte Animation { get; set; } = 0;

        [Field] 
        [Index(7)]
        // TODO: make health dynamic to entity
        public float Health { get; set; } = 20.0f;

        [Field] 
        [Index(8)] 
        public int PotionEffectColor { get; set; } = 0;

        [Field] 
        [Index(9)] 
        public bool IsPotionAmbient { get; set; } = false;

        [Field] 
        [Index(10)] 
        public int Arrows { get; set; } = 0;

        public LivingMetadata(Entity entity) : base(entity)
        {
            
        }
    }
}