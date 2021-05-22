using Trestle.Entities.Enums;
using Trestle.Utils;
using Trestle.Worlds;

namespace Trestle.Entities
{
    public class Entity
    {
        /// <summary>
        /// Unique identifier of the Entity.
        /// </summary>
        public int Id { get; set; } = -1;
        
        /// <summary>
        /// Type of the Entity.
        /// </summary>
        public EntityType Type { get; }

        /// <summary>
        /// World that the Entity is in.
        /// </summary>
        public World World { get; private set; }

        public Location Location { get; set; }
        
        public Entity(EntityType type, World world)
        {
            Type = type;
        }
    }
}