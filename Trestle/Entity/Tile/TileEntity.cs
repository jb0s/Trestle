using fNbt;
using Trestle.Utils;
using Trestle.Worlds;
using System.Collections.Generic;

namespace Trestle.Entity.Tile
{
    public class TileEntity
    {
        public string Id { get; private set; }
        public Vector3 Coordinates { get; set; }
        public bool UpdatesOnTick { get; set; }

        public TileEntity(string id)
        {
            Id = id;
        }

        public virtual NbtCompound GetCompound()
        {
            return new();
        }

        public virtual void SetCompound(NbtCompound compound)
        {
        }

        public virtual void OnTick(World world)
        {
        }

        public virtual List<ItemStack> GetDrops()
        {
            return new();
        }
    }
}