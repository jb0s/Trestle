namespace Trestle.Nbt.Tags
{
    public class NbtLong : NbtTag
    {
        public NbtLong(string name, long value) : base(name, NbtType.Long, value) { }
    }
}