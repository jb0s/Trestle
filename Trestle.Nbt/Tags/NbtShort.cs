namespace Trestle.Nbt.Tags
{
    public class NbtShort : NbtTag
    {
        public NbtShort(string name, short value) : base(name, NbtType.Short, value) { }
    }
}