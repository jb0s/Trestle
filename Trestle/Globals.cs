using System;

namespace Trestle
{
    public class Globals
    {
        public static Random Random;

        public static void Initialize()
        {
            Random = new Random();
        }
    }
}