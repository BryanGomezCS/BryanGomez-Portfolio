using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace Snake.Core
{
    public partial class SnakeGame
    {
        protected override void Update(GameTime gameTime)
        {
            // Allowing exit with button pressed (the x to close the game)
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            switch (m_gameState)
            {
                // The Main Menu case
                case GameState.MainMenu:
                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    {
                        Initialize();
                        m_gameState = GameState.Countdown;
                        CountdownTimer = 3f;
                    }
                    // Dealing with the configuration
                    // For making the animation and hover effect
                    Rectangle gearRect = GetGearRectangle();
                    MouseState mState = Mouse.GetState();

                    // Checking if the spinning effect should start
                    if (m_settingsTransitionTimer <= 0)
                    {
                        bool isHovered = gearRect.Contains(mState.Position);
                        bool isClicked = mState.LeftButton == ButtonState.Pressed;
                        bool isCPressed = Keyboard.GetState().IsKeyDown(Keys.C);

                        if ((isHovered && isClicked) || isCPressed && m_oldMouseState.LeftButton == ButtonState.Released)
                        {
                            m_previousState = GameState.MainMenu;
                            m_settingsTransitionTimer = TRANSITION_DURATION;
                        }

                        // Constant slow spin if just hovering
                        if (isHovered) m_gearRotation += 0.05f;
                    }
                    else
                    {
                        // Doing the transition
                        m_gearRotation += 0.2f;     // fast
                        m_settingsTransitionTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                        // changing screens
                        if (m_settingsTransitionTimer <= 0)
                        {
                            m_gameState = GameState.Configurations;
                        }
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.Q))
                    {
                        Exit();     // ends the game 
                    }
                    // for the "fake" snake to move within the screen
                    m_menuSnakeTimer += gameTime.ElapsedGameTime.TotalSeconds;
                    if (m_menuSnakeTimer >= 0.4) // Slower than the actual game
                    {
                        m_menuSnakeTimer = 0;

                        // Calculating new head position
                        Point newHead = m_menuSnake[0];
                        newHead.X += (int)m_menuSnakeDirection.X;

                        // Screen Wrap Logic (so it never leaves)
                        if (newHead.X >= GameBoard.GRID_WIDTH) newHead.X = 0;
                        if (newHead.X < 0) newHead.X = GameBoard.GRID_WIDTH - 1;

                        // Moving the body
                        m_menuSnake.Insert(0, newHead);
                        m_menuSnake.RemoveAt(m_menuSnake.Count - 1);
                    }

                    // for the leaderboard screen
                    if (Keyboard.GetState().IsKeyDown(Keys.L) && m_oldKeyState.IsKeyUp(Keys.H))
                    {
                        m_isFromGameOver = false;
                        m_gameState = GameState.Leaderboard;

                        // using a scroll effect
                        m_scrollOffset = 0f;
                    }

                    // switching to the How To Play screen
                    if (Keyboard.GetState().IsKeyDown(Keys.H) && m_oldKeyState.IsKeyUp(Keys.H))
                    {
                        m_gameState = GameState.HowToPlay;
                    }
                    break;
                // The leaderboard Case
                case GameState.Leaderboard:
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        m_gameState = GameState.MainMenu;
                    }

                    if (m_isFromGameOver)
                    {
                        if (Keyboard.GetState().IsKeyDown(Keys.Enter) && m_oldKeyState.IsKeyUp(Keys.Enter))
                        {
                            Initialize();
                            m_gameState = GameState.Countdown;
                            CountdownTimer = 3f;
                            m_isFromGameOver = false;
                        }
                    }
                    break;

                // The How To Play case
                case GameState.HowToPlay:
                    // allowing the user to return to the main menu
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape) && m_oldKeyState.IsKeyUp(Keys.Escape))
                    {
                        m_gameState = GameState.MainMenu;
                    }
                    break;

                // The Countdown Case
                case GameState.Countdown:
                    CountdownTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (CountdownTimer <= 0)
                    {
                        m_gameState = GameState.Playing;
                    }
                    break;

                // The Settings Case
                case GameState.Configurations:
                    UpdateConfigurations();
                    break;

                // The Playing Case
                case GameState.Playing:
                    if (Keyboard.GetState().IsKeyDown(Keys.P) && m_oldKeyState.IsKeyUp(Keys.P))
                    {
                        m_gameState = GameState.Paused;
                    }
                    UpdatePlaying(gameTime);
                    break;

                // The Paused Case
                case GameState.Paused:
                    // if P is pressed again, resume the game
                    if (Keyboard.GetState().IsKeyDown(Keys.P) && m_oldKeyState.IsKeyUp(Keys.P))
                    {
                        m_gameState = GameState.Playing;
                    }
                    // or go to the main menu
                    if (Keyboard.GetState().IsKeyDown(Keys.M) && m_oldKeyState.IsKeyUp(Keys.M))
                    {
                        m_gameState = GameState.MainMenu;
                    }
                    // or to go settings
                    if (Keyboard.GetState().IsKeyDown(Keys.C) && m_oldKeyState.IsKeyUp(Keys.C))
                    {
                        m_previousState = GameState.Paused;
                        m_gameState = GameState.Configurations;
                    }
                    // or quit the game
                    if (Keyboard.GetState().IsKeyDown(Keys.Q) && m_oldKeyState.IsKeyUp(Keys.Q))
                    {
                        Exit();
                    }
                    break;

                // The GameOver cases for hitting a wall or the snake hitting itself
                case GameState.GameOver1:
                    UpdateGameOver();
                    break;
                case GameState.GameOver2:
                    UpdateGameOver();
                    break;

                // the case of asking the user/player to save their score with a name
                case GameState.SaveScore:
                    UpdateSaveScore();
                    break;
            }
            m_oldKeyState = Keyboard.GetState();
            m_oldMouseState = Mouse.GetState();

            base.Update(gameTime);
        }

        /// <summary>
        /// Updates the game logic when in the Playing state.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        private void UpdatePlaying(GameTime gameTime)
        {
            HandleInput();

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            m_moveTimer += elapsed;

            if (m_moveTimer >= m_moveDelay)
            {
                // Handling input from queue
                if (m_inputQueue.Count > 0)
                {
                    m_snake.SetDirection(m_inputQueue.Dequeue());
                }

                m_snake.Move();
                m_moveTimer -= m_moveDelay; // This keeps the remainder for smooth motion

                // Collision checks
                if (m_snake.CheckWallCollision(m_gameBoard) || m_snake.CheckSelfCollision())
                {
                    m_gameState = m_snake.CheckWallCollision(m_gameBoard) ? GameState.GameOver1 : GameState.GameOver2;

                    // triggering a flash
                    m_deathFlashTimer = 0.25f;
                }

                // Eating food
                if (m_snake.Head == m_food.Position)
                {
                    m_snake.Grow();
                    m_food.Spawn(m_gameBoard, m_snake);
                    m_score += 25;
                }

                // for the pause
                if (Keyboard.GetState().IsKeyDown(Keys.P) && m_oldKeyState.IsKeyUp(Keys.P))
                {
                    m_gameState = GameState.Paused;     // switching the state to paused but the update will still run and the animation will happen in the Draw method for the paused state
                }

                MouseState mState = Mouse.GetState();
                Rectangle pauseClickArea = new Rectangle(
                    m_graphics.PreferredBackBufferWidth - 120,
                    m_graphics.PreferredBackBufferHeight - 50,
                    120, 50);

                if (mState.LeftButton == ButtonState.Pressed && m_oldMouseState.LeftButton == ButtonState.Released)
                {
                    if (pauseClickArea.Contains(mState.Position))
                    {
                        m_gameState = GameState.Paused;
                    }
                }
            }
        }

        /// <summary>
        /// Updating the Configurations logic
        /// </summary>
        private void UpdateConfigurations()
        {
            KeyboardState kState = Keyboard.GetState();

            if (Keyboard.GetState().IsKeyDown(Keys.Escape) && m_oldKeyState.IsKeyUp(Keys.Escape))
            {
                m_gameState = m_previousState; // Return to the previous state (Main Menu or Paused)
            }

            // for changing themes
            if (kState.IsKeyDown(Keys.D1) && m_oldKeyState.IsKeyUp(Keys.D1))
            {
                m_themeIndex = (m_themeIndex + 1) % ThemeManager.Themes.Count;
            }
            // for making change in speed
            if (kState.IsKeyDown(Keys.D2) && m_oldKeyState.IsKeyUp(Keys.D2))
            {
                // cycle is: normal -> fast -> slow
                if (m_currentSpeed == 0.15)
                {
                    m_currentSpeed = 0.10;
                    m_moveDelay = 0.10f;    // fast
                }
                else if (m_currentSpeed == 0.10)
                {
                    m_currentSpeed = 0.25;
                    m_moveDelay = 0.25f; // slow
                }
                else
                {
                    m_currentSpeed = 0.15;
                    m_moveDelay = 0.15f; // normal
                }
            }
            // for the music
            // A power switch (M Key)
            if (kState.IsKeyDown(Keys.M) && m_oldKeyState.IsKeyUp(Keys.M))
            {
                m_musicEnabled = !m_musicEnabled;

                // if M got pressed Music IS enable, so play the song
                if (m_musicEnabled)
                {
                    MediaPlayer.Play(m_songs[m_currentSongIdx]);
                }
                // if Music is NOT enable than stop the song
                if (!m_musicEnabled)
                {
                    MediaPlayer.Stop();
                }
            }

            // A slider option (Only works if Enabled)
            if (m_musicEnabled)
            {
                if (kState.IsKeyDown(Keys.D4)) // Lower volume
                {
                    m_volume = MathHelper.Clamp(m_volume - 0.01f, 0f, 1f);
                    MediaPlayer.Volume = m_volume;
                }
                if (kState.IsKeyDown(Keys.D5)) // Increase volume
                {
                    m_volume = MathHelper.Clamp(m_volume + 0.01f, 0f, 1f);
                    MediaPlayer.Volume = m_volume;
                }
            }

            // adding the Plalist option
            if (Keyboard.GetState().IsKeyDown(Keys.D3) && m_oldKeyState.IsKeyUp(Keys.D3))
            {
                // Moving to the next song index
                m_currentSongIdx++;

                // If it goes past the last song, loop back to the first one (0)
                if (m_currentSongIdx >= m_songs.Count)
                {
                    m_currentSongIdx = 0;
                }
                // Playing the new song
                MediaPlayer.Play(m_songs[m_currentSongIdx]);
            }
        }

        /// <summary>
        /// Updates the game logic when in the GameOver state.
        /// </summary>
        private void UpdateGameOver()
        {
            KeyboardState kState = Keyboard.GetState();

            // if the user selects Y (Yes) then..
            if (!m_showGameOverOptions)
            {
                if (kState.IsKeyDown(Keys.Y) && m_oldKeyState.IsKeyUp(Keys.Y))
                {
                    m_gameState = GameState.SaveScore; // Go to the typing screen
                    m_showGameOverOptions = false;     // Reset for next death
                }

                if (kState.IsKeyDown(Keys.N) && m_oldKeyState.IsKeyUp(Keys.N))
                {
                    m_showGameOverOptions = true;      // Switch to the "Short Version"
                }
            }
            // If no, show another screen
            else
            {
                if (kState.IsKeyDown(Keys.Enter) && m_oldKeyState.IsKeyUp(Keys.Enter))
                {
                    m_gameState = GameState.Countdown;
                    Initialize();
                    m_showGameOverOptions = false;  // reset
                }
                if (kState.IsKeyDown(Keys.Escape) && m_oldKeyState.IsKeyUp(Keys.Escape))
                {
                    m_gameState = GameState.MainMenu;
                    m_showGameOverOptions = false; // reset
                }
            }
            // Checking if Q is pressed for quitting the game
            if (Keyboard.GetState().IsKeyDown(Keys.Q))
            {
                Exit();
            }
        }

        /// <summary>
        /// Handles the game over option for the user to enter a nickname or not along with their score
        /// </summary>
        private void UpdateSaveScore()
        {
            KeyboardState kState = Keyboard.GetState();

            // making sure only letters in the alphabet are entered
            if (m_playerName.Length < MAX_NAME_LENGTH)
            {
                // checking for only alphabet letters (no special characters allowed)
                for (Keys k = Keys.A; k <= Keys.Z; k++)
                {
                    if (kState.IsKeyDown(k) && m_oldKeyState.IsKeyUp(k))
                    {
                        m_playerName += k.ToString();
                    }
                }
                // Allowing user to enter numbers 
                for (Keys k = Keys.D0; k <= Keys.D9; k++)
                {
                    if (kState.IsKeyDown(k) && m_oldKeyState.IsKeyUp(k))
                    {
                        // .ToString() on Keys.D1 returns "D1", so we strip the 'D'
                        m_playerName += k.ToString().Replace("D", "");
                    }
                }

                // This is the user has a number pad and would like to use it, they can
                for (Keys k = Keys.NumPad0; k <= Keys.NumPad9; k++)
                {
                    if (kState.IsKeyDown(k) && m_oldKeyState.IsKeyUp(k))
                    {
                        m_playerName += k.ToString().Replace("NumPad", "");
                    }
                }
            }
            // Handling backspace
            if (kState.IsKeyDown(Keys.Back) && m_oldKeyState.IsKeyUp(Keys.Back) && m_playerName.Length > 0)
            {
                m_playerName = m_playerName.Substring(0, m_playerName.Length - 1);
            }

            // confirming and finishing
            if (kState.IsKeyDown(Keys.Enter) && m_oldKeyState.IsKeyUp(Keys.Enter))
            {
                string finalName = string.IsNullOrWhiteSpace(m_playerName) ? "GUEST" : m_playerName;
                m_leaderboardManager.AddScore(finalName, m_score);
                m_highScore = Math.Max(m_highScore, m_score);   // ensuring that the local high score is current

                Initialize();
                m_isFromGameOver = true;
                m_gameState = GameState.Leaderboard;    // rewarding the user by showing the list

                m_oldKeyState = kState; // for preventing the "Enter" key from being registered in the leaderboard screen after saving the score
            }
            // The user decided to cancel (not enter a name)
            if (kState.IsKeyDown(Keys.Escape))
            {
                m_gameState = GameState.MainMenu;
            }
        }

        /// <summary>
        /// Handles keyboard input for controlling the snake direction.
        /// </summary>
        private void HandleInput()
        {
            KeyboardState keyState = Keyboard.GetState();
            Snake.Direction? requestedDir = null;

            if (keyState.IsKeyDown(Keys.Up) || keyState.IsKeyDown(Keys.W)) requestedDir = Snake.Direction.Up;
            else if (keyState.IsKeyDown(Keys.Down) || keyState.IsKeyDown(Keys.S)) requestedDir = Snake.Direction.Down;
            else if (keyState.IsKeyDown(Keys.Left) || keyState.IsKeyDown(Keys.A)) requestedDir = Snake.Direction.Left;
            else if (keyState.IsKeyDown(Keys.Right) || keyState.IsKeyDown(Keys.D)) requestedDir = Snake.Direction.Right;

            if (requestedDir.HasValue && m_inputQueue.Count < MAX_BUFFER_SIZE)
            {
                // Getting the "last" direction intended (either currently in the queue or the snake's current heading)
                Snake.Direction lastDir = m_inputQueue.Count > 0 ? m_inputQueue.ToArray()[m_inputQueue.Count - 1] : m_snake.CurrentDirection;

                // ONLY add to queue if it's not a 180-degree turn relative to the LAST intended move
                if (IsOpposite(requestedDir.Value, lastDir)) return;

                // Only add if it's actually a different direction than the last one
                if (requestedDir.Value != lastDir)
                {
                    m_inputQueue.Enqueue(requestedDir.Value);
                }
            }
        }

        // Helper method to keep HandleInput clean
        private bool IsOpposite(Snake.Direction newDir, Snake.Direction currentDir)
        {
            if (newDir == Snake.Direction.Up && currentDir == Snake.Direction.Down) return true;
            if (newDir == Snake.Direction.Down && currentDir == Snake.Direction.Up) return true;
            if (newDir == Snake.Direction.Left && currentDir == Snake.Direction.Right) return true;
            if (newDir == Snake.Direction.Right && currentDir == Snake.Direction.Left) return true;
            return false;

        }

        /// <summary>
        /// A helper function for the gear sprite
        /// </summary>
        private Rectangle GetGearRectangle()
        {
            int padding = 30;
            int gearSize = 40;
            return new Rectangle(
                m_graphics.PreferredBackBufferWidth - gearSize - padding,
                padding,
                gearSize,
                gearSize
            );
        }
    }
}