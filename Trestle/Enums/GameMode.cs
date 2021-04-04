namespace Trestle.Enums
{
    public enum GameMode : byte
    {
        /// <summary>
        /// Survival mode.
        /// </summary>
        Survival,

        /// <summary>
        /// Creative mode. Unlimited blocks and items, flying ability and instant digging.
        /// </summary>
        Creative,

        /// <summary>
        /// Survival mode, but you cannot destroy the world.
        /// </summary>
        Adventure,

        /// <summary>
        /// Ghost mode.
        /// </summary>
        Spectator
    }
}