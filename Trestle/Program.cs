using System;
using Trestle.Networking;

namespace Trestle
{
    class Program
    {
        static void Main(string[] args)
        {
            Globals.Initialize();
            var listener = new Listener();
            listener.Start();
        }
    }
}