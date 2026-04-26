using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Xna.Framework.Media;
using System.Linq;

namespace Snake.Core
{
    /// <summary>
    /// Main game class that manages the game loop, rendering, and game state.
    /// This is the entry point for the MonoGame framework.
    /// </summary>
    public partial class SnakeGame : Game
    {
        #region Fields

        private GraphicsDeviceManager m_graphics;

        /// <summary>
        /// Sprites for rendering 2D graphics.
        /// </summary>
        private SpriteBatch m_spriteBatch;
        /// <summary>
        /// Board for the game
        /// </summary>
        private GameBoard m_gameBoard;
        private Snake m_snake;
        private Food m_food;
        private GameState m_gameState;
        private SpriteFont m_font;
        private SpriteFont m_bigFont;   // A bigger size font
        private SpriteFont m_SmallFont;   // A smaller size font
        private Texture2D m_pixelTexture;
        private double m_currentSpeed = 0.15; // Seconds between snake moves
        private int m_score;
        private Queue<Snake.Direction> m_inputQueue = new Queue<Snake.Direction>();
        private const int MAX_BUFFER_SIZE = 2; // Only store up to 2 moves to keep it responsive  // this is for adding a "delay" to not cause any issues with the Keyboard input

        // Adding the apple, snake head, and snake body since Im using my own sprites
        private Texture2D m_appleTexture;
        private Texture2D m_snakeBodyTexture;
        private Texture2D m_snakeHeadTexture;
        private Texture2D m_snakeTailTexture;
        private Texture2D m_snakeDeadTexture;
        private float m_deathFlashTimer = 0f;

        private Texture2D m_configTexture;  // settings sprite

        // Making the pause to be an arcade style pause so:
        private Texture2D m_pauseTexture;   // for the pause sprite

        private GameState m_previousState; // for remembering the state before pausing (for the pause menu)

        private float CountdownTimer;  // for the countdown process
        private float m_gearRotation = 0f;  // for trying to do an "animation" on the config/settings sprite
        private float m_settingsTransitionTimer = 0f;
        private const float TRANSITION_DURATION = 0.4f; // Fast but noticeable spin
        private float m_moveTimer = 0f;
        private float m_moveDelay = 0.15f; // Adjust for speed (smaller = faster)

        // Implementhing a "fake" snake that would run only in the main menu
        private List<Point> m_menuSnake;
        private double m_menuSnakeTimer;
        private Vector2 m_menuSnakeDirection = new Vector2(1, 0); // Moving Right
        private int m_themeIndex = 0;      // starts with the original "classic mint"
        private KeyboardState m_oldKeyState; // to prevent flickering toggles
        private MouseState m_oldMouseState; // to prevent an issue with pressing the pause sprite with the mouse

        // For adding background music (as a slider)
        private float m_volume = 0.25f; // Starts at 25%

        private bool m_musicEnabled = false; // for a quick ON/OFF and it starts as OFF
        private List<Song> m_songs = new List<Song>();
        private int m_currentSongIdx = 0;

        private int m_highScore = 0;        // for recording a new possible score than a previous one

        // for the leaderboard
        private LeaderboardManager m_leaderboardManager;
        private float m_scrollOffset = 0f;  // for the scrolling effect
        private bool m_showGameOverOptions = false;

        // for the player to be able to add their nickname/name along with the score
        private string m_playerName = "";
        private const int MAX_NAME_LENGTH = 8;
        private bool m_isFromGameOver = false;  // to ask the user if they want to play again AFTER saving their score


        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the Game1 class.
        /// </summary>
        public SnakeGame()
        {
            m_graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // Set window size based on game board dimensions
            m_graphics.PreferredBackBufferWidth = GameBoard.GRID_WIDTH * GameBoard.CELL_SIZE;
            m_graphics.PreferredBackBufferHeight = GameBoard.GRID_HEIGHT * GameBoard.CELL_SIZE + 50; // Extra space for score

            // for having no lag
            m_graphics.SynchronizeWithVerticalRetrace = true; // Enables VSync
            this.IsFixedTimeStep = true;
            this.TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d); // Force 60 FPS
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.
        /// </summary>
        protected override void Initialize()
        {
            m_gameBoard = new GameBoard();
            m_snake = new Snake(GameBoard.GRID_WIDTH / 2, GameBoard.GRID_HEIGHT / 2);
            m_food = new Food();
            m_gameState = GameState.MainMenu;
            m_score = 0;


            // for the "fake" snake
            m_menuSnake = new List<Point>
            {
                new Point(5, 7),
                new Point(4, 7),
                new Point(3, 7)
            };

            // Spawn initial food
            m_food.Spawn(m_gameBoard, m_snake);

            // for the leaderboard
            m_leaderboardManager = new LeaderboardManager();

            // getting the best/highest score so it not 0 every time the game opens/start
            if (m_leaderboardManager.HighScores.Count > 0)
            {
                m_highScore = m_leaderboardManager.HighScores[0].Score;
            }
            else
            {
                m_highScore = 0;
            }

            // 
            m_showGameOverOptions = false;
            m_playerName = "";

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            m_spriteBatch = new SpriteBatch(GraphicsDevice);

            // Creating a 1x1 white pixel texture for drawing rectangles
            m_pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            m_pixelTexture.SetData(new[] { Color.White });

            // Loading the sprite drawings
            m_appleTexture = Content.Load<Texture2D>("Sprites/apple");
            m_snakeHeadTexture = Content.Load<Texture2D>("Sprites/snake_head");
            m_snakeBodyTexture = Content.Load<Texture2D>("Sprites/snake_body");
            m_snakeTailTexture = Content.Load<Texture2D>("Sprites/snake_tail");
            m_snakeDeadTexture = Content.Load<Texture2D>("Sprites/snake_collide");
            m_pauseTexture = Content.Load<Texture2D>("Sprites/pause_btn");
            m_configTexture = Content.Load<Texture2D>("Sprites/settings");

            // for background music
            foreach (var track in MusicManager.Playlist)
            {
                m_songs.Add(Content.Load<Song>(track.Audio));
            }

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = m_volume;

            m_font = Content.Load<SpriteFont>("Fonts/Hud");
            m_bigFont = Content.Load<SpriteFont>("Fonts/BigFont");
            m_SmallFont = Content.Load<SpriteFont>("Fonts/SmallFont");
        }
        #endregion
    }
}
