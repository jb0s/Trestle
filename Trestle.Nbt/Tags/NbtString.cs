using System.IO;

namespace Trestle.Nbt.Tags
{
    public class NbtString : NbtTag
    {
        public NbtString(string name, string value) : base(name, NbtType.String, value) { }
    }
}