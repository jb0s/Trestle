using System.Threading.Tasks;
using Trestle.Entities.Players;
using Trestle.Levels.Enums;

namespace Trestle.Levels.Items
{
    public interface IEdibleItem : IItem
    {
        /// <summary>
        /// How many food points to grant the player upon consumption.
        /// Max is 20.
        /// </summary>
        public int FoodPoints => 20;

        /// <summary>
        /// How much saturation to give to the player upon consumption.
        /// Saturation determines how fast the hunger level depletes.
        /// Max is the player's FoodLevel value.
        /// </summary>
        public int SaturationPoints => 5;
        
        public async Task<InteractionResult> Use(Player player, PlayerHand hand)
            => InteractionResult.Consume;
    }
}