using System;
using System.Collections;
using System.Collections.Generic;

namespace Trestle.Nbt.Tags
{
    public class NbtCompound : NbtTag, ICollection<NbtTag>
    {
        public List<NbtTag> Tags { get; } = new ();

        public int Count 
            => Tags.Count;
        
        public bool IsReadOnly 
            => false;
        
        public NbtCompound(string name = "") : base(name, NbtType.Compound)
            => Value = Tags;

        public void Add(NbtTag item)
            => Tags.Add(item);

        public void Clear()
            => Tags.Clear();

        public bool Contains(NbtTag item)
            => Tags.Contains(item);

        public void CopyTo(NbtTag[] array, int arrayIndex)
            => Tags.CopyTo(array, arrayIndex);

        public bool Remove(NbtTag item)
            => Tags.Remove(item);

        public IEnumerator<NbtTag> GetEnumerator()
            => throw new NotImplementedException();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}