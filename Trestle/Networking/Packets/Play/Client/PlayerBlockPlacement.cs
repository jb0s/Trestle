using System;
using Trestle.Attributes;
using Trestle.Blocks;
using Trestle.Enums;
using Trestle.Utils;

namespace Trestle.Networking.Packets.Play.Client
{
    [IgnoreExceptions]
    [ServerBound(PlayPacket.Server_PlayerBlockPlacement)]
    public class PlayerBlockPlacement : Packet
    {
        [Field]
        public Vector3 Location { get; set; }

        [Field] 
        public byte Face { get; set; }
        
        [Field]
        [VarInt]
        public int Hand { get; set; }
        
        [Field]
        public byte CursorX { get; set; }
        
        [Field]
        public byte CursorY { get; set; }
        
        [Field]
        public byte CursorZ { get; set; }

        public override void HandlePacket()
        {
            try
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

                if (existingBlock.IsUsable && !Client.Player.IsCrouching)
                {
                    existingBlock.UseItem(Client.Player.World, Client.Player, Location, (BlockFace)Face);
                    return;
                }

                var itemInHand = Client.Player.Inventory.GetItemInHand((PlayerHand)Hand);

                Client.Player.World.SetBlock(itemInHand.Material, newLocation);
                Client.SendPacket(new SoundEffect($"dig.{itemInHand.Material.ToString().ToLower()}", Client.Player.Location.ToVector3()));
                
                if (Client.Player.GameMode != GameMode.Creative)
                    Client.Player.Inventory.RemoveItem((short)itemInHand.Id, itemInHand.Metadata, 1);
            }
            catch
            {
            }
        }
    }
}