namespace Trestle.Nbt.Tags
{
    public class NbtDouble : NbtTag
    {
        public NbtDouble(string name, double value) : base(name, NbtType.Double, value) { }
    }
}