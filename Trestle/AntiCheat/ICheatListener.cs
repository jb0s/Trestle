using System.Threading.Tasks;
using Trestle.Entity;

namespace Trestle.AntiCheat
{
    public interface ICheatListener
    {
        public Task Listen(Player player);
        public Task OnTriggered(Player player);
    }
}