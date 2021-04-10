using System;
using Trestle.Entity;
using Trestle.Enums;
using Trestle.Networking.Packets.Play.Client;
using Trestle.Utils;

namespace Trestle.Block.Blocks
{
    public class Workbench : Block
    {
        public Workbench() : base(Material.Workbench)
        {
            IsUsable = true;
        }

        public override void UseItem(World.World world, Player player, Vector3 blockCoordinates, BlockFace face)
        {
        }
    }
}