namespace Trestle.Utils
{
    public class Velocity
    {
        public short X { get; set; }
        
        public short Y { get; set; }
        
        public short Z { get; set; }

        public Velocity(short x, short y, short z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}