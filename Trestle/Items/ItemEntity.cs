using Trestle.Networking.Packets.Play.Client;
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

        public override void SpawnEntity()
        {
            base.SpawnEntity();

            foreach (var player in World.Players.Values)
            {
                var spawnedBy = player.Client;
                spawnedBy.SendPacket(new SpawnObject(2, (int)Location.X, (int)Location.Y, (int)Location.Z, 0, 0, 1));
            }
        }

        public override void OnTick()
        {
        }
    }
}