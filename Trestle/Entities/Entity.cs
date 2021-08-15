using Trestle.Entities.Enums;
using Trestle.Levels;
using Trestle.Utils;

namespace Trestle.Entities
{
    public class Entity
    {
        /// <summary>
        /// Unique identifier of the Entity.
        /// </summary>
        public readonly int Id = -1;

        /// <summary>
        /// Type of the Entity.
        /// </summary>
        public EntityType Type { get; }

        /// <summary>
        /// Level that the Entity is in.
        /// </summary>
        public Level Level { get; }

        public Location Location { get; set; } = new(0, 100, 0);
        
        public Entity(EntityType type, Level level)
        {
            Type = type;
            Level = level;
        }
    }
}