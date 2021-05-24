namespace Trestle.Levels.Enums
{
    public enum InteractionResult
    {
        /// <summary>
        /// The interaction was successful.
        /// </summary>
        Success,
        
        /// <summary>
        /// The player consumes an item.
        /// </summary>
        Consume,
        
        /// <summary>
        /// Allows the client to vanilla handle this interaction.
        /// Any other InteractionResult skips this.
        /// </summary>
        Pass,
        
        /// <summary>
        /// The interaction failed.
        /// </summary>
        Fail
    }
}