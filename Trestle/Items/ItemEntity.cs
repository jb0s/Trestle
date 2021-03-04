using Trestle.Utils;
using Trestle.Worlds;

namespace Trestle.Entity
{
    public class ItemEntity : Entity
    {
        public ItemStack Item { get; private set; }
        public int PickupDelay { get; set; }
        public int TimeToLive { get; set; }

        public ItemEntity(World world, ItemStack item) : base(2, world)
        {
            Item = item;

            PickupDelay = 10;
            TimeToLive = 6000;
        }

        private void DespawnEntity(Player source)
        {
            World.RemoveEntity(this);
        }

        public override void SpawnEntity()
        {
            World.AddEntity(this);
        }

        public override void OnTick()
        {
        }
    }
}