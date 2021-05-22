using System.IO;

namespace Trestle.Nbt.Tags
{
    public class NbtByte : NbtTag
    {
        public NbtByte(string name, byte value) : base(name, NbtType.Byte, value) { }
    }
}