using System;
using Trestle.Enums;
using Trestle.Entity;
using System.Collections;

namespace Trestle.Utils
{
    public class Item
    {
        public ushort Id { get; set; }
        public byte Metadata { get; set; }
        public bool IsUsable { get; set; }
        public int MaxStackSize { get; set; }
        public bool IsBlock { get; set; }
        public Material Material
        {
            get => (Material)Id;
            set => Id = (ushort)value;
        }
        
        public Item() {}
        
        internal Item(ushort id, byte metadata)
        {
            Id = id;
            Metadata = metadata;

            MaxStackSize = 64;
            IsUsable = false;
            IsBlock = false;
        }
        
        public virtual void UseItem(World.World world, Player player, Vector3 blockCoordinates, BlockFace face)
        {
        }
        
        public byte ConvertToByte(BitArray bits)
        {
            if (bits.Count != 8)
            {
                throw new ArgumentException("bits");
            }
            byte[] bytes = new byte[1];
            bits.CopyTo(bytes, 0);
            return bytes[0];
        }
    }
}