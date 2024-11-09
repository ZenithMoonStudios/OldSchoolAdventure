using Destiny;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace OSA
{
    /// <summary>
    /// Level intro screen class.
    /// </summary>
    public class LevelIntroScreen : PlayScreen
    {
        /// <summary>
        /// Data.
        /// </summary>
        Level m_Level;
        int m_PageIndex;

        /// <summary>
        /// Textures.
        /// </summary>
        Texture2D m_BackgroundTexture;
        Texture2D m_ButtonTexture;
        Vector2 m_BackgroundTexturePosition;

        /// <summary>
        /// Fonts.
        /// </summary>
        SpriteFont m_Font;
        int m_FontHeight;

        /// <summary>
        /// Constraints.
        /// </summary>
        float m_Speed;

        /// <summary>
        /// State.
        /// </summary>
        int m_RevealedLineCount = 0;
        int m_RevealedLineCharacters = 0;
        string m_CurrentLine = string.Empty;

        /// <summary>
        /// Accessors.
        /// </summary>
        LevelIntroPage Page { get { return m_Level.IntroPages[m_PageIndex]; } }
        bool AllLinesRevealed { get { return m_RevealedLineCount >= this.Page.Lines.Count; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        public LevelIntroScreen(Level p_Level) : this(p_Level, 0) { }
        public LevelIntroScreen(Level p_Level, int p_PageIndex)
        {
            m_Level = p_Level;
            m_PageIndex = p_PageIndex;
            m_Speed = 0.5f;

            this.ContentRelativeFolder = "Level Intro Screen";
            this.TransitionOnTime = TimeSpan.FromMilliseconds(1000);
            this.TransitionOffTime = TimeSpan.FromMilliseconds(1000);
        }

        public override void LoadContent()
        {
            // Load content manager, etc.
            base.LoadContent();

            // Load resources
            m_BackgroundTexture = this.ContentManager.Load<Texture2D>("DialogBackground");
            m_BackgroundTexturePosition.X = (this.Viewport.Width - m_BackgroundTexture.Width) / 2;
            m_BackgroundTexturePosition.Y = (this.Viewport.Height - m_BackgroundTexture.Height) / 2;
            m_ButtonTexture = this.ContentManager.Load<Texture2D>("Button");

            m_Font = this.ContentManager.Load<SpriteFont>("Dialog");
            m_FontHeight = (int)m_Font.MeasureString("W").Y - 2;

            // Once the load has finished, we need to tell the game that we have just
            // finished a very long frame, and that it should not try to catch up
            this.ScreenManager.Game.ResetElapsedTime();
        }

        #region Input

        /// <summary>
        /// Handle input.
        /// Only called when screen has focus.
        /// </summary>
        /// <param name="p_InputState">Input state.</param>
        public override void HandleInput(InputState p_InputState)
        {
            base.HandleInput(p_InputState);

            if (p_InputState.IsAccept())
            {
                // Play audio
                this.AudioManager.PlayCue("MenuSelect");

                if (!this.AllLinesRevealed)
                {
                    // Not all text has been revealed, so reveal it
                    m_RevealedLineCount = this.Page.Lines.Count;
                    m_CurrentLine = string.Empty;
                }
                else
                {
                    if (m_PageIndex == m_Level.IntroPages.Count - 1)
                    {
                        // Load play screen
                        LoadingScreen.Load(this.ScreenManager, true, new LevelPlayScreen(m_Level));
                    }
                    else
                    {
                        // Go to the next page
                        this.ScreenManager.AddScreen(new LevelIntroScreen(m_Level, m_PageIndex + 1));
                        this.ExitScreen();
                    }
                }
            }
        }

        //private static bool IsNext(InputState p_InputState)
        //{
        //    return p_InputState.IsNewKeyPress(Keys.Space) || p_InputState.IsNewButtonPress(Buttons.A);
        //}

        #endregion

        #region Update

        /// <summary>
        /// Update screen.
        /// Called regardless of screen state.
        /// </summary>
        /// <param name="p_GameTime">Game time.</param>
        /// <param name="p_HasFocus">Whether screen has focus.</param>
        /// <param name="p_IsCoveredByOtherScreen">Whether screen is covered by another screen.</param>
        public override void Update(GameTime p_GameTime, bool p_HasFocus, bool p_IsCoveredByOtherScreen)
        {
            // Call base class to handle transitions
            base.Update(p_GameTime, p_HasFocus, p_IsCoveredByOtherScreen);

            if (p_HasFocus)
            {
                // Work out how much to reveal, based on how much time has passed
                if (!this.AllLinesRevealed)
                {
                    int totalCharacterCount = (int)(m_Speed * this.ActiveFrames);
                    int currentLineCharacterCount = totalCharacterCount - m_RevealedLineCharacters;

                    // Only process text if additional characters should be added
                    if (currentLineCharacterCount > m_CurrentLine.Length)
                    {
                        // Multiple lines could be added if speed is fast enough, so keep going
                        // until all current line characters are exhausted
                        while (currentLineCharacterCount > 0)
                        {
                            string currentLine = this.Page.Lines[m_RevealedLineCount];
                            int currentLineLength = currentLine.Length;
                            if (currentLineCharacterCount >= currentLineLength)
                            {
                                // Current line is finished, so add it to revealed lines
                                m_RevealedLineCount++;
                                m_RevealedLineCharacters += currentLineLength;
                                currentLineCharacterCount -= currentLineLength;
                                m_CurrentLine = string.Empty;

                                // If this is the last line, we're finished
                                if (m_RevealedLineCount == this.Page.Lines.Count)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                m_CurrentLine = currentLine.Substring(0, currentLineCharacterCount);
                                currentLineCharacterCount = 0;
                            }
                        }
                    }
                }
            }
        }

        #endregion

        static Vector2 s_TempVector = Vector2.Zero;
        public override void Draw(GameTime p_GameTime)
        {
            // Start sprite batch
            SpriteBatch spriteBatch = this.SpriteBatch;
            spriteBatch.Begin();

            // Calculate alpha for fade in and out
            Color backgroundColor = ColorHelper.AddAlpha(Color.White, (byte)((int)this.TransitionAlpha * 0.7f));
            Color textColor = ColorHelper.AddAlpha(Color.Black, this.TransitionAlpha);

            // Draw the dialog background
            spriteBatch.Draw(m_BackgroundTexture, m_BackgroundTexturePosition, backgroundColor);

            // Draw the text
            s_TempVector.X = m_BackgroundTexturePosition.X + 25;
            s_TempVector.Y = m_BackgroundTexturePosition.Y + 12;
            for (int index = 0; index < m_RevealedLineCount; index++)
            {
                spriteBatch.DrawString(m_Font, this.Page.Lines[index], s_TempVector, textColor);
                s_TempVector.Y = s_TempVector.Y + m_FontHeight;
            }
            if (!string.IsNullOrEmpty(m_CurrentLine))
            {
                spriteBatch.DrawString(m_Font, m_CurrentLine, s_TempVector, textColor);
            }

            // Draw the continue prompt
            if (this.AllLinesRevealed)
            {
                s_TempVector.X = m_BackgroundTexturePosition.X + m_BackgroundTexture.Width - 50;
                s_TempVector.Y = m_BackgroundTexturePosition.Y + m_BackgroundTexture.Height - 50;
                Color buttonColor = ColorHelper.AddAlpha(Color.White, this.TransitionAlpha);
                spriteBatch.Draw(m_ButtonTexture, s_TempVector, buttonColor);
            }

            // End sprite batch
            spriteBatch.End();
        }

    }
}
