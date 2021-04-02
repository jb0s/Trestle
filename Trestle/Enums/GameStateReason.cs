namespace Trestle.Enums
{
    public enum GameStateReason : byte
    {
        /// <summary>
        /// Your bed was missing or obstructed.
        /// </summary>
        InvalidBed = 0,
        
        /// <summary>
        /// Ends rain.
        /// </summary>
        EndRaining = 1,
        
        /// <summary>
        /// Starts rain.
        /// </summary>
        BeginRaining = 2,
        
        /// <summary>
        /// Change the player's game mode.
        /// 0 = Survival, 1 = Creative, 2 = Adventure, 3 = Spectator
        /// </summary>
        ChangeGameMode = 3, 
        
        /// <summary>
        /// Enter endgame credits.
        /// </summary>
        EnterCredits = 4,
        
        /// <summary>
        /// Show the demo window.
        /// 0 = Welcome message, 101 = movement controls, 102 = jump controls, 103 = inventory controls
        /// </summary>
        DemoMessage = 5,
        
        /// <summary>
        /// "Ding" sound effect when you hit a player with an arrow.
        /// </summary>
        ArrowHittingPlayer = 6,
        
        /// <summary>
        /// Changes ambient color.
        /// 1 = dark, 0 = bright, setting the value any higher causes weird colors along with a freeze.
        /// </summary>
        FadeValue = 7,
        
        /// <summary>
        /// Time in ticks for the sky to fade.
        /// </summary>
        FadeTime = 8,
        
        /// <summary>
        /// ???
        /// </summary>
        PlayMobAppearance = 10
    }
}