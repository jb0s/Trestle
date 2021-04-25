using Trestle.Entity;
using Trestle.Networking;
using System.Collections.Generic;
using Trestle.AntiCheat.Listeners;

namespace Trestle.AntiCheat
{
    public class TrestleAntiCheat
    {
        public List<ICheatListener> Listeners { get; private set; } = new();

        public TrestleAntiCheat()
        {
            //Listeners.Add(new AirJumpListener());
        }
        
        public void OnTick()
        {
            foreach (var player in TrestleServer.GetOnlinePlayers())
                foreach (var listener in Listeners)
                    listener.Listen(player);
        }
    }
}