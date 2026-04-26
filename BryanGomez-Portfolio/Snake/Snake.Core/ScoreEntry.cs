using System;

namespace Snake.Core
{
    /// <summary>
    /// Represents a high score entry with the score and player name/data.
    /// </summary>
    [Serializable]
    public struct ScoreEntry
    {
        public string PlayerName;
        public int Score;
        public DateTime Date;

        // Constructor to initialize the score entry
        public ScoreEntry(string name, int score)
        {
            PlayerName = name;
            Score = score;
            Date = DateTime.Now;
        }
    }
}