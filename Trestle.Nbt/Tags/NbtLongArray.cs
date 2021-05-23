using System;
using System.Collections;
using System.Collections.Generic;

namespace Trestle.Nbt.Tags
{
    public class NbtLongArray : NbtTag, ICollection<long>
    {
        public List<long> Tags { get; } = new ();
        
        public int Count 
            => Tags.Count;
        
        public bool IsReadOnly 
            => false;
        
        public NbtLongArray(string name = "") : base(name, NbtType.LongArray)
            => Value = Tags;

        public void Add(long item)
            => Tags.Add(item);

        public void Clear()
            => Tags.Clear();

        public bool Contains(long item)
            => Tags.Contains(item);

        public void CopyTo(long[] array, int arrayIndex)
            => Tags.CopyTo(array, arrayIndex);

        public bool Remove(long item)
            => Tags.Remove(item);

        public IEnumerator<long> GetEnumerator()
            => Tags.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}