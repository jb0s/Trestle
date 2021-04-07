using System;
using Trestle.Attributes;
using Trestle.Blocks;
using Trestle.Enums;
using Trestle.Enums.Packets.Server;
using Trestle.Networking.Packets.Play.Client;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Play.Server
{
    [IgnoreExceptions]
    [ServerBound(PlayPacket.PlayerBlockPlacement)]
    public class PlayerBlockPlacement : Packet
    {
        [Field]
        public Vector3 Location { get; set; }

        [Field] 
        [VarInt]
        public int Face { get; set; }
        
        [Field]
        [VarInt]
        public int Hand { get; set; }
        
        [Field]
        public float CursorX { get; set; }
        
        [Field]
        public float CursorY { get; set; }
        
        [Field]
        public float CursorZ { get; set; }

        public override void HandlePacket()
        {
            var newLocation = Location;
            var existingBlock = Client.Player.World.GetBlock(Location);

            switch (Face)
            {
                case 0:
                    newLocation.Y--;
                    break;

                case 1:
                    newLocation.Y++;
                    break;

                case 2:
                    newLocation.Z--;
                    break;

                case 3:
                    newLocation.Z++;
                    break;

                case 4:
                    newLocation.X--;
                    break;

                case 5:
                    newLocation.X++;
                    break;
            }

            if (existingBlock.IsUsable)
            {
                existingBlock.UseItem(Client.Player.World, Client.Player, Location, (BlockFace)Face);
                return;
            }

            var itemInHand = Client.Player.Inventory.GetItemInHand();

            Client.Player.World.SetBlock(newLocation, itemInHand.Material);
                
            if (Client.Player.GameMode != GameMode.Creative)
                Client.Player.Inventory.RemoveItem((short)itemInHand.Id, itemInHand.Metadata, 1);
        }
    }
}