namespace Trestle.Nbt.Tags
{
    public class NbtInt : NbtTag
    {
        public NbtInt(string name, int value) : base(name, NbtType.Int, value) { }
    }
}