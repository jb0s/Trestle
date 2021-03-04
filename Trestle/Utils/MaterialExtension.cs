using Trestle.Enums;

namespace Trestle.Utils
{
    public static class MaterialExtension
    {
        public static Material GetMaterial(this int id)
            => (Material) id;
    }
}