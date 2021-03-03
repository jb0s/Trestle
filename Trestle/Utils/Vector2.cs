namespace Trestle.Utils
{
    public class Vector2
    {
        public Vector2(int x, int z)
        {
            X = x;
            Z = z;
        }

        public int X { get; set; }
        public int Z { get; set; }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X - b.X, a.Z - b.Z);
        }
    }
}