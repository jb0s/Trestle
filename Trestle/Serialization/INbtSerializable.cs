using fNbt;

namespace Trestle.Serialization
{
    public interface INbtSerializable
    {
        NbtTag Serialize(string tagName);
        void Deserialize(NbtTag value);
    }
}