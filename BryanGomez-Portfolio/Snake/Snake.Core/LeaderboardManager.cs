using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Snake.Core
{
    // A class of the leaderboard
    public class LeaderboardManager
    {
        private const string FILE_NAME = "Leaderboard.xml";
        private List<ScoreEntry> m_scores;
        public List<ScoreEntry> HighScores => m_scores;

        // Constructor of the leaderboard
        public LeaderboardManager()
        {
            m_scores = new List<ScoreEntry>();
            LoadScores();
        }

        // A method to add scores 
        public void AddScore(string name, int score)
        {
            m_scores.Add(new ScoreEntry(name, score));
            // sorting form highest to lowest
            m_scores = m_scores.OrderByDescending(s => s.Score).Take(10).ToList();  // only keeping the top 10 scores
            SaveScores();
        }

        // A method to Save Scores
        public void SaveScores()
        {
            // using xml serializer to save the scores to a file
            XmlSerializer serializer = new XmlSerializer(typeof(List<ScoreEntry>));
            using (StreamWriter writer = new StreamWriter(FILE_NAME))
            {
                serializer.Serialize(writer, m_scores);
            }
        }

        // A method to load the scores
        public void LoadScores()
        {
            if (!File.Exists(FILE_NAME)) return;

            XmlSerializer serializer = new XmlSerializer(typeof(List<ScoreEntry>));
            using (StreamReader reader = new StreamReader(FILE_NAME))
            {
                m_scores = (List<ScoreEntry>)serializer.Deserialize(reader);
            }
        }
    }
}