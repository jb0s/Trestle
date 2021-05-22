namespace Trestle.Nbt.Tags
{
    public class NbtFloat : NbtTag
    {
        public NbtFloat(string name, float value) : base(name, NbtType.Float, value) { }
    }
}