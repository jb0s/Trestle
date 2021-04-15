using System;
using System.IO;
using System.Reflection;
using fNbt;
using fNbt.Tags;
using Trestle.Attributes;
using Trestle.Enums;
using Trestle.Utils;

namespace Trestle.World
{
    /// <summary>
    /// Extra data for a block, to store text, items, etc.
    /// </summary>
    public class BlockEntity
    {
        /// <summary>
        /// Id (or type) of the block entity
        /// </summary>
        public BlockEntityType Id { get; set; }

        /// <summary>
        /// Determines if the block entity is invalidated
        /// </summary>
        public bool KeepPacked { get; set; } = false;

        /// <summary>
        /// Position of the block
        /// </summary>
        public Vector3 Position { get; set; }
        
        /// <summary>
        /// Extra data
        /// </summary>
        public NbtCompound Data { get; set; }

        public BlockEntity(BlockEntityType id, Vector3 position, NbtCompound data)
        {
            Id = id;
            Position = position;
            Data = data;
        }
        
        public BlockEntity(NbtCompound data)
        {
            // TODO: dynamic plz
            Id = BlockEntityType.Chest;

            var x = data.Get<NbtInt>("x")?.Value ?? 0;
            var y = data.Get<NbtInt>("y")?.Value ?? 0;
            var z = data.Get<NbtInt>("z")?.Value ?? 0;

            Position = new Vector3(x, y, z);

            data.Remove("x");
            data.Remove("y");
            data.Remove("z");
            data.Remove("keepPacked");
            data.Remove("id");

            Data = data;
        }
        
        /// <summary>
        /// Exports all the Nbt data to a <c>byte[]</c>
        /// </summary>
        /// <returns></returns>
        public byte[] Export()
        {
            // suffers
            var id = Id.GetType().GetMember(Id.ToString())[0].GetCustomAttribute<DescriptionAttribute>()?.Description;
            
            var compound = new NbtCompound("")
            {
                new NbtString("id", id),
                new NbtByte("keepPacked", (byte)(KeepPacked ? 0 : 1)),
                new NbtInt("x", (int)Position.X),
                new NbtInt("y", (int)Position.Y),
                new NbtInt("z", (int)Position.Z)
            };

            var stream = new MemoryStream();
            new NbtFile(compound).SaveToStream(stream, NbtCompression.None);
            
            return stream.ToArray();
        }
    }
}