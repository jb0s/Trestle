namespace Trestle.Utils
{
    public struct Location
    {
        public double X { get; set; }
        
        public double Y { get; set; }
        
        public double Z { get; set; }
        
        public float Yaw { get; set; }
        
        public float Pitch { get; set; }
        
        public bool OnGround { get; set; }
        
        public Location(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
            Yaw = 0;
            Pitch = 0;
            OnGround = false;
        }
    }
}