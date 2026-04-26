namespace Snake.Core
{
    /// <summary>
    /// Represents the possible states of the game.
    /// </summary>
    public enum GameState
    {
        /// <summary>
        /// The game is currently being played.
        /// </summary>
        MainMenu,
        /// <summary>
        /// The game is about to start.
        /// </summary>
        Configurations,
        /// <summary>
        /// The game is about to start.
        /// </summary>
        Countdown,
        /// <summary>
        /// The game is currently being played.
        /// </summary>
        Playing,

        /// <summary>
        /// The game is paused.
        /// </summary>
        Paused,

        /// <summary>
        /// The game has ended (snake hitself or hit the wall) and asks the user if they want to save their score.
        /// </summary>
        GameOver1,

        GameOver2,
        Leaderboard,
        SaveScore,  // where the user can enter their name with their score
        HowToPlay,  // To help a new player how the game works
    }
}
