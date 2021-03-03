namespace Trestle.Entity
{
    public class EntityManager
    {
        private static int _entityId;

        public static int GetEntityId()
        {
            return _entityId;
            _entityId++;
        }
    }
}