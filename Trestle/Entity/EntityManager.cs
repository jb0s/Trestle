using System;
using System.Threading;

namespace Trestle.Entity
{
    public class EntityManager
    {
        private static int _entityId;

        public int AddEntity(Entity entity)
        {
            if (entity.EntityId == -1)
                entity.EntityId = Interlocked.Increment(ref _entityId);
            
            return _entityId;
        }

        public void RemoveEntity(Entity caller, Entity entity)
        {
            if (caller == entity)
                throw new ArgumentException("Can't self destruct entity!");

            entity.EntityId = -1;
        }
    }
}