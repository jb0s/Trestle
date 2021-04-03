using Trestle.Entity;
using Trestle.Enums;
using Trestle.Utils;
using Trestle.Worlds;

namespace Trestle.Blocks
{
    public class Block : Item
    {
        internal Block(Material material) : this((ushort)material)
        {
        }

        internal Block(ushort id) : base(id, 0)
        {
            Id = id;
            Durability = 0.5f;
            Metadata = 0;
            Drops = new[] {new ItemStack(this, 1)};
			
            IsSolid = true;
            IsBuildable = true;
            IsReplacible = false;
            IsTransparent = false;

            FuelEfficiency = 0;
            IsBlock = true;
        }

        public Vector3 Coordinates { get; set; }
        public bool IsReplacible { get; set; }
        public bool IsSolid { get; set; }
        public bool IsTransparent { get; set; }
        public float Durability { get; set; }
        public ItemStack[] Drops { get; set; }
        public bool IsBuildable { get; set; }

        public void DoDrop(World world)
        {
            if (Drops == null) return;
            foreach (var its in Drops)
            {
                new ItemEntity(world, its)
                {
                    Location = new Location(Coordinates.X, Coordinates.Y + 0.25, Coordinates.Z)
                }.SpawnEntity();
            }
        }
        
        public float GetHardness()
        {
            return Durability / 5f;
        }

        public virtual void OnTick(World world)
        {
        }

        public virtual void DoPhysics(World world)
        {
        }

        public void BreakBlock(World playerWorld)
        {
            playerWorld.SetBlock(Material.Air, Coordinates);
        }
    }
}