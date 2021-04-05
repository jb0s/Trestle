using Trestle.Entity;
using Trestle.Networking;

namespace Trestle.Commands
{
    public class Command
    {
        public Client Client { get; set; }

        public Player Player => Client.Player;
    }
}