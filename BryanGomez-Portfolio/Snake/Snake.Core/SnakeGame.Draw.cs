using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Snake.Core
{
    public partial class SnakeGame
    {
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // for changing the colors along with the theme
            var theme = ThemeManager.Themes[m_themeIndex];

            GraphicsDevice.Clear(theme.Background);
            m_spriteBatch.Begin(samplerState: SamplerState.LinearClamp);

            // Drawing each Game State
            if (m_gameState == GameState.MainMenu)
            {
                DrawMainMenu(theme.Text);
            }
            else if (m_gameState == GameState.Leaderboard)
            {
                DrawLeaderboard(theme.Text);
            }
            else if (m_gameState == GameState.Countdown)
            {
                DrawCountdown();
            }
            else if (m_gameState == GameState.Configurations)
            {
                DrawConfigurations();
            }
            else if (m_gameState == GameState.HowToPlay)
            {
                DrawHowToPlay();
            }
            else if (m_gameState == GameState.Playing)
            {
                DrawPlaying(theme.Text);
                DrawPauseHUD(theme.Text);
            }
            else if (m_gameState == GameState.Paused)
            {
                DrawPaused(theme.Text);
            }
            else if (m_gameState == GameState.GameOver1)
            {
                DrawGameOver(theme.Text, "You Hit The Wall!");
            }
            else if (m_gameState == GameState.GameOver2)
            {
                DrawGameOver(theme.Text, "You Crash Into Yourself!");
            }
            else if (m_gameState == GameState.SaveScore)
            {
                DrawSaveScore(theme.Text);
            }

            // adding a flash effect once the snake collides
            if (m_deathFlashTimer > 0)
            {
                float alpha = m_deathFlashTimer / 0.2f; // Fades out
                m_spriteBatch.Draw(m_pixelTexture,
                    new Rectangle(0, 0, m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight),
                    Color.Red * (alpha * 0.5f)); // 50% opacity red flash

                m_deathFlashTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            m_spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Draws the Main Menu when in the Main Menu state and when the game opens 
        /// </summary>
        private void DrawMainMenu(Color textColor)
        {
            var theme = ThemeManager.Themes[m_themeIndex];

            string m_Title = "HUNGRY SNAKE";
            string m_Prompt = "Press Enter to Start!";

            // Calculating positions to center the text perfectly
            Vector2 titleSize = m_bigFont.MeasureString(m_Title);
            Vector2 prompSize = m_font.MeasureString(m_Prompt);

            Vector2 titlePos = new Vector2(
                (int)((m_graphics.PreferredBackBufferWidth - titleSize.X) / 2),
                (int)((m_graphics.PreferredBackBufferHeight / 2) - 90)
                );

            Vector2 promptPos = new Vector2(
                (m_graphics.PreferredBackBufferWidth - prompSize.X) / 2,
                titlePos.Y + 90);

            // Showing the title
            m_spriteBatch.DrawString(m_bigFont, m_Title, titlePos, theme.Text);
            // Drawing the instruction
            m_spriteBatch.DrawString(m_font, m_Prompt, promptPos, textColor);

            // Adding the option to quit the game
            string quitPrompt = "[Q] Quit Game";
            int padding = 20;   // to "find" the left side on the X

            Vector2 quitSize = m_font.MeasureString(quitPrompt);
            Vector2 quitPos = new Vector2(
                padding,
                m_graphics.PreferredBackBufferHeight - quitSize.Y + 20 - padding
            );

            m_spriteBatch.DrawString(m_SmallFont, quitPrompt, quitPos, textColor);

            // Calculating the top right corner 
            int gear_Padding = 30;
            int gearSize = 40; // making the sprite be the right size
            Rectangle gearRect = new Rectangle(
                m_graphics.PreferredBackBufferWidth - gearSize - gear_Padding,
                padding,
                gearSize,
                gearSize
            );

            // need an origin to rotate around the center of the gear
            Vector2 gearOrigin = new Vector2(m_configTexture.Width / 2f, m_configTexture.Height / 2f);

            // To rotate around the center, the Position needs to be the center of the rect
            Vector2 gearCenter = new Vector2(gearRect.X + gearSize / 2, gearRect.Y + gearSize / 2);

            m_spriteBatch.Draw(
                m_configTexture,
                gearCenter,
                null,
                Color.White,
                m_gearRotation, // This variable handles the animation
                gearOrigin,
                new Vector2((float)gearSize / m_configTexture.Width), 
                SpriteEffects.None,
                0f
            );

            // Adding the text under it
            string settingsHint = "[C] Configs";
            m_spriteBatch.DrawString(m_SmallFont, settingsHint, new Vector2(gearRect.X - 65, gearRect.Y + 45), theme.Text);

            // Adding the leaderboard text
            string leaderboardPrompt = "[L] View Leaderboard";
            Vector2 leaderSize = m_SmallFont.MeasureString(leaderboardPrompt);
            Vector2 leaderPos = new Vector2(
                (m_graphics.PreferredBackBufferWidth - leaderSize.X - 250) / 2,
                promptPos.Y + 80 // Placed below the "Start" prompt
            );
            m_spriteBatch.DrawString(m_SmallFont, leaderboardPrompt, leaderPos, Color.OrangeRed * 0.6f);

            // Adding a How To Play for new users
            string howToPrompt = "[H] How To Play";
            Vector2 howToSize = m_SmallFont.MeasureString(howToPrompt);
            Vector2 howToPos = new Vector2(
                (m_graphics.PreferredBackBufferWidth - howToSize.X + 300) / 2,
                promptPos.Y + 80
            );
            m_spriteBatch.DrawString(m_SmallFont, howToPrompt, howToPos, Color.OrangeRed * 0.6f);

            // drawing the "fake" snake
            for (int i = 0; i < m_menuSnake.Count; i++)
            {
                Texture2D segmentTexture;
                float rotation = 0f;

                // Determining which texture to use
                if (i == 0) // The Head
                {
                    segmentTexture = m_snakeHeadTexture;
                    // Directional rotation for the menu snake
                    if (m_menuSnakeDirection.X > 0) rotation = 0f;
                    else if (m_menuSnakeDirection.X < 0) rotation = MathHelper.Pi;
                    else if (m_menuSnakeDirection.Y > 0) rotation = MathHelper.PiOver2;
                    else if (m_menuSnakeDirection.Y < 0) rotation = -MathHelper.PiOver2;
                }
                else if (i == m_menuSnake.Count - 1) // The Tail
                {
                    segmentTexture = m_snakeTailTexture;
                    // Tail points toward the segment in front of it (i-1)
                    rotation = GetRotationRadians(m_menuSnake[i], m_menuSnake[i - 1]) + MathHelper.PiOver2;
                }
                else // The Body
                {
                    segmentTexture = m_snakeBodyTexture;
                    rotation = GetRotationRadians(m_menuSnake[i], m_menuSnake[i - 1]) + MathHelper.PiOver2;
                }

                // Calculating Screen Position (Centered in the cell)
                Vector2 screenPos = new Vector2(
                    m_menuSnake[i].X * GameBoard.CELL_SIZE + (GameBoard.CELL_SIZE / 2f),
                    m_menuSnake[i].Y * GameBoard.CELL_SIZE + (GameBoard.CELL_SIZE / 2f)
                );

                Vector2 origin = new Vector2(segmentTexture.Width / 2f, segmentTexture.Height / 2f);

                m_spriteBatch.Draw(
                    segmentTexture,
                    screenPos,
                    null,
                    Color.White * 0.6f, // Keeping a little light
                    rotation,
                    origin,
                    (float)GameBoard.CELL_SIZE / segmentTexture.Width,
                    SpriteEffects.None,
                    0f
                );
            }

        }

        /// <summary>
        /// Draws the leaderboard screen 
        /// </summary>
        private void DrawLeaderboard(Color textColor)
        {
            var theme = ThemeManager.Themes[m_themeIndex];

            string title = "TOP HUNGRY SNAKES";
            m_spriteBatch.DrawString(m_bigFont, title, new Vector2((m_graphics.PreferredBackBufferWidth - m_bigFont.MeasureString(title).X) / 2, 50), Color.Gold);

            // Header for the list
            string header = "RANK  SCORE  NAME             DATE";
            m_spriteBatch.DrawString(m_font, header, new Vector2(100, 120), theme.Text);

            // Creating a "Clipping Rectangle" so scores don't overlap the title
            // (Or just start drawing lower)
            var scores = m_leaderboardManager.HighScores;
            int startY = 170;

            for (int i = 0; i < scores.Count; i++)
            {
                // Calculate Y based on scroll
                float yPos = startY + (i * 40) - m_scrollOffset;

                // Only draw if it's on screen
                if (yPos > 150 && yPos < m_graphics.PreferredBackBufferHeight - 100)
                {
                    Color rowColor = (i == 0) ? Color.Gold : (i == 1) ? Color.Silver : (i == 2) ? Color.BurlyWood : theme.Text;
                    // Format the date to be short (e.g., 03/24/26)
                    string dateStr = scores[i].Date.ToString("MM/dd/yy");

                    // Using spaces or tabs to align (or use specific X coordinates for columns)
                    string rank = $"#{i + 1}";
                    string score = scores[i].Score.ToString("D4");
                    string name = scores[i].PlayerName.PadRight(10); // Pad to keep columns straight

                    m_spriteBatch.DrawString(m_font, rank, new Vector2(120, yPos), rowColor);
                    m_spriteBatch.DrawString(m_font, score, new Vector2(200, yPos), rowColor);
                    m_spriteBatch.DrawString(m_font, name, new Vector2(300, yPos), rowColor);
                    m_spriteBatch.DrawString(m_font, dateStr, new Vector2(490, yPos), rowColor * 0.9f);
                }
            }

            // Update Scroll Logic
            m_scrollOffset += 0.7f;
            if (m_scrollOffset > (scores.Count * 40)) m_scrollOffset = -100f; // Loop back

            // asking the user if they want to play again
            if (m_isFromGameOver)
            {
                string playAgain = "PRESS [ENTER] TO PLAY AGAIN";
                Vector2 againSize = m_font.MeasureString(playAgain);
                Vector2 pos = new Vector2((m_graphics.PreferredBackBufferWidth - againSize.X) / 2, m_graphics.PreferredBackBufferHeight - 80);

                // adding a flickering effect
                Color flashColor = (DateTime.Now.Millisecond < 500) ? Color.Gold : Color.Black;

                m_spriteBatch.DrawString(m_font, playAgain, pos, flashColor);
            }

            // Footer
            string footer = "Press [Esc] for Menu";
            m_spriteBatch.DrawString(m_SmallFont, footer, new Vector2(20, m_graphics.PreferredBackBufferHeight - 40), Color.Gray);
        }

        /// <summary>
        /// Draws the How To Play screen for first time players
        /// </summary>
        private void DrawHowToPlay()
        {
            var theme = ThemeManager.Themes[m_themeIndex];
            Vector2 startPos = new Vector2(200, 25);
            Vector2 textPos = new Vector2(5, 70);

            m_spriteBatch.DrawString(m_bigFont, "How To Play", startPos, theme.Text);
            m_spriteBatch.DrawString(m_font, "- Use Arrow Keys or AWSD to move the snake.", textPos + new Vector2(0, 80), theme.Text);
            m_spriteBatch.DrawString(m_font, "- Eat the apple to grow and gain points.", textPos + new Vector2(0, 120), theme.Text);
            m_spriteBatch.DrawString(m_font, "- Don't hit the walls or yourself!", textPos + new Vector2(0, 160), theme.Text);
            m_spriteBatch.DrawString(m_SmallFont, "Can You Grow the Snake Until There Is No More Space left?", textPos + new Vector2(20, 280), Color.DodgerBlue * 0.7f);

            m_spriteBatch.DrawString(m_SmallFont, "Press [Esc] to go back", new Vector2(5, 510), Color.DarkGray);
        }

        /// <summary>
        /// Draws the game countdown whe in the Countdown State
        /// </summary>
        private void DrawCountdown()
        {
            var theme = ThemeManager.Themes[m_themeIndex];
            // using ceiling to not get decimal numbers in the countdown
            string displayValue = Math.Ceiling(CountdownTimer).ToString();
            Vector2 size = m_bigFont.MeasureString(displayValue);
            Vector2 screenCenter = new Vector2(m_graphics.PreferredBackBufferWidth / 2, m_graphics.PreferredBackBufferHeight / 2);

            m_spriteBatch.DrawString(m_bigFont, displayValue, screenCenter - (size / 2), theme.Text);
        }

        /// <summary>
        /// Draws the game settings/configurations where the player can make some modifications
        /// </summary>
        private void DrawConfigurations()
        {
            var theme = ThemeManager.Themes[m_themeIndex];

            // Header
            m_spriteBatch.DrawString(m_bigFont, "CONFIGURATIONS", new Vector2(100, 50), theme.Text);

            // Change Themes
            string themeText = $"1. Current Theme: {theme.Name}";
            string changeTheme = "Press (1) to change the theme";
            m_spriteBatch.DrawString(m_font, themeText, new Vector2(50, 150), theme.Text);
            m_spriteBatch.DrawString(m_SmallFont, changeTheme, new Vector2(80, 180), theme.Text);

            // Change of Speed
            string speedName = m_currentSpeed == 0.10 ? "Fast" : (m_currentSpeed == 0.25 ? "Slow" : "Normal");
            string speedText = $"2. Snake Speed: [{speedName}] - Press (2)";
            m_spriteBatch.DrawString(m_font, speedText, new Vector2(50, 230), theme.Text);

            // For the music
            string musicStatus = m_musicEnabled ? "[ON]" : "[OFF]";
            m_spriteBatch.DrawString(m_font, $"3. Music: {musicStatus} - Press (M)", new Vector2(50, 300), theme.Text);

            // Adding the Plalist
            if (m_musicEnabled)
            {
                var trackInfo = MusicManager.Playlist[m_currentSongIdx];

                m_spriteBatch.DrawString(m_font, $"Current Song: [{trackInfo.displayName}]", new Vector2(80, 335), theme.Text);

                // how to change the song
                string changeSong = "Press (3) to change the song";
                m_spriteBatch.DrawString(m_SmallFont, changeSong, new Vector2(80, 400), theme.Text);

                // adding the credits depending the song being played
                string artist = "";
                if (trackInfo.displayName == "Hype") artist = "Next Route";
                else if (trackInfo.displayName == "Retro") artist = "moodmode";
                else if (trackInfo.displayName == "Chill") artist = "Johny Grimes";

                m_spriteBatch.DrawString(m_SmallFont, $"Artist: {artist}", new Vector2(80, 375), Color.Gray);

                // Slider (Visual feedback)
                Color sliderColor = m_musicEnabled ? Color.LimeGreen : Color.Gray * 0.3f;
                Color sliderLabelColor = m_musicEnabled ? theme.Text : Color.Gray;

                m_spriteBatch.DrawString(m_SmallFont, $"Volume: {(int)(m_volume * 100)}% (Press 4 or 5)", new Vector2(445, 375), sliderLabelColor);

                // Bar Background
                Rectangle barBG = new Rectangle(500, 360, 200, 10);
                m_spriteBatch.Draw(m_pixelTexture, barBG, Color.Black * 0.3f);

                // Bar Fill
                Rectangle barFill = new Rectangle(500, 360, (int)(200 * m_volume), 10);
                m_spriteBatch.Draw(m_pixelTexture, barFill, sliderColor);
            }

            // Footer to tell the user how to return to the Main Menu
            string footer = (m_previousState == GameState.Paused) ? "Press [Esc] for Resume" : "Press [Esc] for Main Menu";
            m_spriteBatch.DrawString(m_SmallFont, footer, new Vector2(20, m_graphics.PreferredBackBufferHeight - 40), theme.Text);
        }

        /// <summary>
        /// Draws the game when in the Playing state.
        /// </summary>
        private void DrawPlaying(Color textColor)
        {
            var theme = ThemeManager.Themes[m_themeIndex];
            // Draw game board grid
            DrawGrid(theme.GridLine);

            // Drawing the Apple (food)
            m_spriteBatch.Draw(m_appleTexture,
                new Rectangle(m_food.Position.X * GameBoard.CELL_SIZE,
                              m_food.Position.Y * GameBoard.CELL_SIZE,
                              GameBoard.CELL_SIZE, GameBoard.CELL_SIZE),
                Color.White);

            // Calculating the interpolation factor (how far through the move we are)
            float t = m_moveTimer / m_moveDelay;

            var fullBody = m_snake.FullBody;
            var oldBody = m_snake.OldFullBody;

            // Safety check: if game just started, use current body as old body
            if (oldBody == null) oldBody = fullBody;

            for (int i = 0; i < fullBody.Count; i++)
            {
                Vector2 currentGridPos = new Vector2(fullBody[i].X, fullBody[i].Y);
                Vector2 previousGridPos = new Vector2(oldBody[i].X, oldBody[i].Y);
                Vector2 smoothGridPos = Vector2.Lerp(previousGridPos, currentGridPos, t);

                Vector2 screenPos = new Vector2(
                    smoothGridPos.X * GameBoard.CELL_SIZE + (GameBoard.CELL_SIZE / 2f),
                    smoothGridPos.Y * GameBoard.CELL_SIZE + (GameBoard.CELL_SIZE / 2f)
                );

                Texture2D currentTexture;
                float targetRot;
                float startRot;

                Color segmentTint = (i == 0) ? theme.SnakeHead : theme.SnakeBody;

                if (i == 0) // THE HEAD
                {
                    // tinting the snake based on the theme

                    if (m_gameState == GameState.GameOver1 || m_gameState == GameState.GameOver2)
                    {
                        segmentTint = Color.White;
                    }

                    currentTexture = (m_gameState == GameState.GameOver1 || m_gameState == GameState.GameOver2)
                                     ? m_snakeDeadTexture : m_snakeHeadTexture;

                    targetRot = GetRotationRadians(m_snake.CurrentDirection);
                    startRot = GetRotationRadians(m_snake.PreviousDirection);
                }
                else // THE BODY OR TAIL
                {
                    currentTexture = (i == fullBody.Count - 1) ? m_snakeTailTexture : m_snakeBodyTexture;

                    // Target rotation based on current frame
                    targetRot = GetRotationRadians(fullBody[i], fullBody[i - 1]) + MathHelper.PiOver2;

                    // Start rotation based on previous frame
                    startRot = GetRotationRadians(oldBody[i], oldBody[i - 1]) + MathHelper.PiOver2;
                }


                // WrapAngle ensures the snake doesn't spin 270 degrees the wrong way when turning
                float diff = MathHelper.WrapAngle(targetRot - startRot);
                float smoothRotation = startRot + (diff * t);

                Vector2 currentOrigin = new Vector2(currentTexture.Width / 2f, currentTexture.Height / 2f);

                m_spriteBatch.Draw(
                currentTexture,
                screenPos,
                null,
                segmentTint, // Use the theme color here!
                smoothRotation,
                currentOrigin,
                (float)GameBoard.CELL_SIZE / currentTexture.Width * 1.05f,
                SpriteEffects.None,
                0f
            );
            }
            // Draw score
            DrawScore(textColor);
        }
        // Version for the HEAD: Converts the Direction enum to radians
        private float GetRotationRadians(Snake.Direction dir)
        {
            return dir switch
            {
                Snake.Direction.Right => 0f,
                Snake.Direction.Down => MathHelper.PiOver2,    // 90 degrees
                Snake.Direction.Left => MathHelper.Pi,         // 180 degrees
                Snake.Direction.Up => -MathHelper.PiOver2,     // 270 degrees (or 3*Pi/2)
                _ => 0f
            };
        }
        // Version for the BODY/TAIL: Figures out rotation by looking at the segment in front
        private float GetRotationRadians(Point current, Point next)
        {
            if (next.X > current.X) return 0f;                    // Moving Right
            if (next.Y > current.Y) return MathHelper.PiOver2;    // Moving Down
            if (next.X < current.X) return MathHelper.Pi;         // Moving Left
            if (next.Y < current.Y) return -MathHelper.PiOver2;   // Moving Up
            return 0f;
        }


        /// <summary>
        /// Draws the pause screen overlay when in the Paused state
        /// </summary>
        private void DrawPaused(Color textColor)
        {
            // drawing the game in the backgroung
            DrawPlaying(textColor);

            // a faded overlay
            m_spriteBatch.Draw(m_pixelTexture,
            new Rectangle(0, 0, m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight),
            Color.Black * 0.7f);

            // using math for a jump effect 
            // Math.Sin takes radians and returns a value between -1 and 1
            float time = (float)DateTime.Now.TimeOfDay.TotalSeconds; // Using total seconds for a continuously increasing value
            float speed = 4f;       // How fast it bobs
            float amplitude = 15f;  // How high it jumps (in pixels)
            float jumpOffset = (float)Math.Sin(time * speed) * amplitude;

            // drawing text on screen
            string pauseText = "GAME PAUSED";
            Vector2 pauseSize = m_bigFont.MeasureString(pauseText);
            Vector2 p_pos = new Vector2((m_graphics.PreferredBackBufferWidth - pauseSize.X) / 2,
                                (m_graphics.PreferredBackBufferHeight - pauseSize.Y) / 2 + jumpOffset) + new Vector2(0, -90); // Slightly above center to account for the jump

            m_spriteBatch.DrawString(m_bigFont, pauseText, p_pos, Color.Yellow);

            // making subtext 
            string opt1 = "PRESS [P] TO RESUME";
            string opt2 = "PRESS [M] FOR MAIN MENU";
            string opt3 = "PRESS [C] FOR CONFIGURATIONS";
            string opt4 = "PRESS [Q] TO QUIT GAME";

            Vector2 size1 = m_SmallFont.MeasureString(opt1);
            Vector2 size2 = m_SmallFont.MeasureString(opt2);
            Vector2 size3 = m_SmallFont.MeasureString(opt3);
            Vector2 size4 = m_SmallFont.MeasureString(opt4);

            m_spriteBatch.DrawString(m_SmallFont, opt1, new Vector2((m_graphics.PreferredBackBufferWidth - size1.X) / 2, p_pos.Y + 120), Color.LawnGreen);
            m_spriteBatch.DrawString(m_SmallFont, opt2, new Vector2((m_graphics.PreferredBackBufferWidth - size2.X) / 2, p_pos.Y + 150), Color.Beige);
            m_spriteBatch.DrawString(m_SmallFont, opt3, new Vector2((m_graphics.PreferredBackBufferWidth - size3.X) / 2, p_pos.Y + 180), Color.DodgerBlue);
            m_spriteBatch.DrawString(m_SmallFont, opt4, new Vector2((m_graphics.PreferredBackBufferWidth - size4.X) / 2, p_pos.Y + 210), Color.Red);


        }

        // making a pause HUD to be added in the DrawPlaying on the bottom right
        private void DrawPauseHUD(Color textColor)
        {
            // Setting up manual values
            float spriteTargetSize = 40f; // making the sprite to be the right size


            // This creates a multiplier to shrink/grow the texture to exactly 40px
            float spriteScale = spriteTargetSize / m_pauseTexture.Width;


            // Positioning the Sprite first
            Vector2 spritePos = new Vector2(
                m_graphics.PreferredBackBufferWidth - spriteTargetSize,
                m_graphics.PreferredBackBufferHeight - spriteTargetSize
            );

            // Positioning the Text to the left of the sprite
            string label = "[P] PAUSE";
            Vector2 textSize = m_SmallFont.MeasureString(label);
            Vector2 textPos = new Vector2(
                spritePos.X - textSize.X - 11, // 11px gap between text and sprite
                spritePos.Y + (spriteTargetSize / 2) - (textSize.Y / 2) // Centering text vertically with sprite
            );

            m_spriteBatch.DrawString(m_SmallFont, label, textPos, textColor);

            if (m_pauseTexture != null)
            {
                m_spriteBatch.Draw(
                    m_pauseTexture,
                    spritePos,
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    spriteScale,
                    SpriteEffects.None,
                    0f
                );
            }
        }

        /// <summary>
        /// Draws the game over screen when hit with a wall
        /// </summary>
        private void DrawGameOver(Color textColor, string reason)
        {
            // Drawing the game in the background
            DrawPlaying(textColor);

            // Drawing a semi-transparent "Dimmer" overlay
            // This makes the UI stand out from the game board
            m_spriteBatch.Draw(m_pixelTexture,
                new Rectangle(0, 0, m_graphics.PreferredBackBufferWidth, m_graphics.PreferredBackBufferHeight),
                Color.Black * 0.7f);


            int centerX = m_graphics.PreferredBackBufferWidth / 2;  // for determining the positions for the things below

            // Header: GAME OVER (Top)
            string header = "GAME OVER";
            Vector2 headerSize = m_bigFont.MeasureString(header);
            m_spriteBatch.DrawString(m_bigFont, header,
                new Vector2(centerX - (headerSize.X / 2), 100), Color.Red);

            // Sub-header: The Reason (Below Header)
            Vector2 reasonSize = m_font.MeasureString(reason);
            m_spriteBatch.DrawString(m_font, reason,
                new Vector2(centerX - (reasonSize.X / 2), 180), Color.White);

            // Score Display
            string scoreText = $"FINAL SCORE: {m_score}";
            string bestScore = $"PERSONAL BEST: {m_highScore}";

            // adding a flash effect whenever the score is broken
            bool isFlashOn = DateTime.Now.Millisecond < 500;
            Color setColor;

            // if the score is broken
            if (m_score >= m_highScore && m_score > 0)
            {
                setColor = isFlashOn ? Color.Gold : Color.GhostWhite;
            }
            else
            {
                // showing a normal score
                setColor = Color.Yellow;
            }

            m_spriteBatch.DrawString(m_font, scoreText,
                new Vector2(centerX - (m_font.MeasureString(scoreText).X / 2), 250), setColor);
            m_spriteBatch.DrawString(m_font, bestScore,
                new Vector2(centerX - (m_font.MeasureString(bestScore).X / 2), 300), Color.LightGray);


            if (!m_showGameOverOptions)
            {
                // Save Prompt 
                string prompt = (m_score >= m_highScore && m_score > 0)
                ? "NEW HIGH SCORE! SAVE TO LEADERBOARD? [Y/N]"
                : "SAVE THIS SCORE? [Y/N]";

                Vector2 promptSize = m_SmallFont.MeasureString(prompt);
                m_spriteBatch.DrawString(m_SmallFont, prompt,
                    new Vector2(centerX - (promptSize.X / 2), 380), Color.DeepSkyBlue * 0.8f);
            }
            else
            {
                // Navigation Options if N is pressed
                string opt1 = "[ENTER] RESTART";
                string opt2 = "[ESC] MAIN MENU";
                string opt3 = "[Q] QUIT GAME";

                m_spriteBatch.DrawString(m_font, opt1, new Vector2(centerX - (m_font.MeasureString(opt1).X / 2), 350), Color.DodgerBlue * 0.9f);
                m_spriteBatch.DrawString(m_font, opt2, new Vector2(centerX - (m_font.MeasureString(opt2).X / 2), 400), Color.LightBlue * 0.9f);
                m_spriteBatch.DrawString(m_font, opt3, new Vector2(centerX - (m_font.MeasureString(opt3).X / 2), 450), Color.Red * 0.8f);
            }


        }

        /// <summary>
        /// Draws the screen when the game ends to either add a name to the score if the player/user wants to
        /// </summary>
        private void DrawSaveScore(Color textColor)
        {
            var theme = ThemeManager.Themes[m_themeIndex];
            string title = "RECORD YOUR SCORE";
            string subtitle = "Type your name or make it up!";
            string nameDisplay = m_playerName + ((DateTime.Now.Millisecond < 500) ? "_" : "");

            // Center the text
            Vector2 titleSize = m_bigFont.MeasureString(title);
            Vector2 subSize = m_font.MeasureString(subtitle);
            Vector2 nameSize = m_bigFont.MeasureString(nameDisplay);

            m_spriteBatch.DrawString(m_bigFont, title, new Vector2((m_graphics.PreferredBackBufferWidth - titleSize.X) / 2, 100), Color.Gold);
            m_spriteBatch.DrawString(m_font, subtitle, new Vector2((m_graphics.PreferredBackBufferWidth - subSize.X) / 2, 180), theme.Text);
            m_spriteBatch.DrawString(m_bigFont, nameDisplay, new Vector2((m_graphics.PreferredBackBufferWidth - nameSize.X) / 2, 240), theme.Text);

            m_spriteBatch.DrawString(m_font, $"FINAL SCORE: {m_score}", new Vector2(250, 350), theme.Text);
            m_spriteBatch.DrawString(m_SmallFont, "PRESS [ENTER] TO CONFIRM", new Vector2(70, 450), Color.Blue);
            m_spriteBatch.DrawString(m_SmallFont, "PRESS [Esc] TO CANCEL", new Vector2(460, 450), Color.Red * 0.85f);
        }

        /// <summary>
        /// Draws the current score at the top of the screen.
        /// </summary>
        private void DrawScore(Color textColor)
        {
            if (m_font != null)
            {
                string scoreText = $"Score: {m_score}";
                Vector2 scorePosition = new Vector2(10, GameBoard.GRID_HEIGHT * GameBoard.CELL_SIZE + 15);
                m_spriteBatch.DrawString(m_font, scoreText, scorePosition, textColor);
            }
        }

        /// <summary>
        /// Draws a grid to visualize the game board cells.
        /// </summary>
        private void DrawGrid(Color textColor)
        {
            // Draw vertical lines
            for (int x = 0; x <= GameBoard.GRID_WIDTH; x++)
            {
                Rectangle line = new Rectangle(
                    x * GameBoard.CELL_SIZE,
                    0,
                    1,
                    GameBoard.GRID_HEIGHT * GameBoard.CELL_SIZE
                );
                m_spriteBatch.Draw(m_pixelTexture, line, textColor * 0.7f);
            }

            // Draw horizontal lines
            for (int y = 0; y <= GameBoard.GRID_HEIGHT; y++)
            {
                Rectangle line = new Rectangle(
                    0,
                    y * GameBoard.CELL_SIZE,
                    GameBoard.GRID_WIDTH * GameBoard.CELL_SIZE,
                    1
                );
                m_spriteBatch.Draw(m_pixelTexture, line, textColor * 0.7f);
            }
        }


        /// <summary>
        /// Draws a single cell on the game board at the specified position.
        /// </summary>
        /// <param name="position">The grid position of the cell.</param>
        /// <param name="color">The color to draw the cell.</param>
        private void DrawCell(Point position, Color color)
        {
            Rectangle cellRect = new Rectangle(
                position.X * GameBoard.CELL_SIZE + 1,
                position.Y * GameBoard.CELL_SIZE + 1,
                GameBoard.CELL_SIZE - 2,
                GameBoard.CELL_SIZE - 2
            );
            m_spriteBatch.Draw(m_pixelTexture, cellRect, color);
        }
    }
}