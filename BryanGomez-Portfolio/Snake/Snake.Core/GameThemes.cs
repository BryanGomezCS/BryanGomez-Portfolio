using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Snake.Core
{
    public struct GameTheme
    {
        public string Name;
        public Color Background;
        public Color GridLine;
        public Color SnakeBody;
        public Color SnakeHead;
        public Color Text;
    }

    public static class ThemeManager
    {
        public static List<GameTheme> Themes = new List<GameTheme>
        {
            new GameTheme {
                Name = "Classic Mint",
                Background = new Color(230, 255, 230),
                GridLine = new Color(200, 230, 200),
                SnakeBody = Color.LightGreen,
                SnakeHead = Color.LightGreen,
                Text = Color.Black
            },
            new GameTheme {
                Name = "Retro Arcade",
                Background = new Color(10, 10, 35),
                GridLine = new Color(30, 30, 80),
                SnakeBody = new Color(0, 255, 255),
                SnakeHead = new Color(0, 150, 255),
                Text = new Color(255, 0, 255)
            },
            new GameTheme {
                Name = "Wasteland",
                Background = new Color(25, 25, 20),
                GridLine = new Color(45, 45, 30),
                SnakeBody = Color.Orange,
                SnakeHead = Color.Orange,
                Text = new Color(200, 255, 0)
            },
            new GameTheme {
                Name = "Lava Pit",
                Background = new Color(40, 10, 10),
                GridLine = new Color(80, 20, 20),
                SnakeBody = new Color(255, 100, 0),
                SnakeHead = new Color(255, 50, 0),
                Text = new Color(255, 200, 0)
            }
        };
    }
}