using Destiny;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace OSA
{
    /// <summary>
    /// Play conversation screen class.
    /// </summary>
    public class PlayConversationScreen : PlayScreen
    {
        /// <summary>
        /// Play screen.
        /// </summary>
        LevelPlayScreen m_PlayScreen;

        /// <summary>
        /// Textures.
        /// </summary>
        Texture2D m_SpeakerBackgroundTexture;
        Texture2D m_BackgroundTexture;
        Texture2D m_ButtonTexture;

        /// <summary>
        /// Fonts.
        /// </summary>
        SpriteFont m_DialogFont;
        int m_DialogFontHeight;

        /// <summary>
        /// Conversation.
        /// </summary>
        SpeakerConversation m_Conversation;

        /// <summary>
        /// Constraints.
        /// </summary>
        float m_Speed;

        /// <summary>
        /// State.
        /// </summary>
        int m_SentenceIndex;
        SpeakerConversationSentence m_CurrentSentence;
        int m_SentenceElapsed;
        bool m_IsTextFullyRevealed;

        string m_FullText;
        string m_FullText1;
        string m_Text1;
        string m_Text2;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="p_PlayScreen">Play screen.</param>
        public PlayConversationScreen(LevelPlayScreen p_PlayScreen, SpeakerConversation p_Conversation)
        {
            m_PlayScreen = p_PlayScreen;
            m_Conversation = p_Conversation;
            m_Speed = 0.5f;

            m_SentenceIndex = 0;

            this.ContentRelativeFolder = "Play Conversation Screen";
            this.TransitionOnTime = TimeSpan.FromMilliseconds(200);
            this.TransitionOffTime = TimeSpan.FromMilliseconds(200);
            this.IsPopup = true;

            this.EnabledGestures = Microsoft.Xna.Framework.Input.Touch.GestureType.Tap;

        }

        #region Load and Unload content

        /// <summary>
        /// Load content.
        /// </summary>
        public override void LoadContent()
        {
            // Load content manager, etc.
            base.LoadContent();

            // Load textures
            m_SpeakerBackgroundTexture = TextureHelper.CreateUnitTexture(Color.LightGreen, this.Game.GraphicsDevice);
            m_BackgroundTexture = this.ContentManager.Load<Texture2D>("Background");
            m_ButtonTexture = this.ContentManager.Load<Texture2D>("Button");
            m_DialogFont = this.ContentManager.Load<SpriteFont>("Dialog");
            m_DialogFontHeight = (int)m_DialogFont.MeasureString("W").Y - 2;

            if (m_Conversation.Sentences.Count == 0)
            {
                // If no more sentences, finish conversation
                m_Conversation.ExecuteTransactions(m_PlayScreen.MainCharacter.Store);
                this.ExitScreen();
            }

            // Once the load has finished, we need to tell the game that we have just
            // finished a very long frame, and that it should not try to catch up
            this.ScreenManager.Game.ResetElapsedTime();
        }

        #endregion

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

                if (!m_IsTextFullyRevealed)
                {
                    // If text not fully revealed, reveal it
                    m_IsTextFullyRevealed = true;
                }
                else
                {
                    // Go to the next sentence
                    m_SentenceIndex++;
                    if (m_SentenceIndex >= m_Conversation.Sentences.Count)
                    {
                        // If no more sentences, finish conversation
                        m_Conversation.ExecuteTransactions(m_PlayScreen.MainCharacter.Store);
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
                this.UpdateState();
            }
        }

        /// <summary>
        /// Update conversation state.
        /// </summary>
        private void UpdateState()
        {
            bool isHandled = false;

            m_SentenceElapsed++;
            if (m_SentenceIndex < m_Conversation.Sentences.Count)
            {
                // Detect if new sentence
                SpeakerConversationSentence sentence = m_Conversation.Sentences[m_SentenceIndex];
                bool isSentenceChanged = (sentence != m_CurrentSentence);
                if (isSentenceChanged)
                {
                    m_CurrentSentence = sentence;
                    m_FullText = m_CurrentSentence.Content;
                    m_SentenceElapsed = 0;
                    m_IsTextFullyRevealed = false;
                    m_FullText1 = string.Empty;

                    // Determine if text needs to be split
                    if (m_FullText.Contains("\\n"))
                    {
                        m_FullText1 = m_FullText.Substring(0, m_FullText.IndexOf("\\n"));
                        m_FullText = m_FullText.Replace("\\n", " ");
                    }
                    else if (m_DialogFont.MeasureString(m_FullText).X > m_BackgroundTexture.Width - 136)
                    {
                        string[] words = m_FullText.Split(' ');
                        string text = string.Empty;
                        for (int i = 0; i < words.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(text))
                            {
                                text += " ";
                            }
                            text += words[i];
                            if (m_DialogFont.MeasureString(text).X > m_BackgroundTexture.Width - 136)
                            {
                                break;
                            }
                            m_FullText1 = text;
                        }
                    }
                }

                // Work out how much text to reveal, based on how much time has passed
                string revealedText = string.Empty;
                if (m_IsTextFullyRevealed)
                {
                    revealedText = m_FullText;
                }
                else
                {
                    int textLength = (int)(m_Speed * m_SentenceElapsed);
                    if (textLength >= m_FullText.Length)
                    {
                        m_IsTextFullyRevealed = true;
                        textLength = m_FullText.Length;
                    }
                    revealedText = m_FullText.Substring(0, textLength);
                }

                // Set the text lines
                if (string.IsNullOrEmpty(m_FullText1) || revealedText.Length <= m_FullText1.Length)
                {
                    m_Text1 = revealedText;
                    m_Text2 = string.Empty;
                }
                else
                {
                    m_Text1 = m_FullText1;
                    m_Text2 = revealedText.Substring(m_FullText1.Length + 1);
                }

                isHandled = true;
            }

            if (!isHandled)
            {
                m_Text1 = string.Empty;
                m_Text2 = string.Empty;
                m_FullText1 = string.Empty;
                m_IsTextFullyRevealed = false;
            }
        }

        #endregion

        #region Draw

        /// <summary>
        /// Draw screen.
        /// </summary>
        static Vector2 s_DrawVector;
        public override void Draw(GameTime p_GameTime)
        {
            if (m_Conversation.Sentences.Count == 0 || m_PlayScreen.DrawManager == null)
            {
                return;
            }

            // Start sprite batch
            SpriteBatch spriteBatch = this.SpriteBatch;
            spriteBatch.Begin();

            // Calculate alpha for fade in and out
            byte backgroundAlpha = (byte)((int)this.TransitionAlpha * 0.8);
            byte foregroundAlpha = this.TransitionAlpha;

            int referenceX = (this.Viewport.Width - m_BackgroundTexture.Width) / 2;
#if MOBILE
			int referenceY = this.Viewport.Height - Convert.ToInt32(1.1f * m_BackgroundTexture.Height);
#else
            int referenceY = Convert.ToInt32((0.9f * this.Viewport.Height) - (1.1f * m_BackgroundTexture.Height));
#endif

            // Draw the speaker
            if (m_CurrentSentence != null && m_CurrentSentence is SpeakerConversationSentence)
            {
                SpeakerConversationSentence sentence = m_CurrentSentence as SpeakerConversationSentence;
                m_PlayScreen.DrawManager.DrawObject(
                    sentence.SpeakerObjectTypePath, referenceX + m_BackgroundTexture.Width - 92, referenceY + 12, false,
                    "Speak", m_SentenceElapsed, m_PlayScreen.Camera, ColorHelper.AddAlpha(Color.White, foregroundAlpha), spriteBatch
                    );
            }

            // Draw the dialog background
            s_DrawVector.X = referenceX;
            s_DrawVector.Y = referenceY;
            spriteBatch.Draw(m_BackgroundTexture, s_DrawVector, ColorHelper.AddAlpha(Color.White, backgroundAlpha));

            // Draw the text
            s_DrawVector.X = referenceX + 22;
            s_DrawVector.Y = referenceY + 15;
            if (!string.IsNullOrEmpty(m_Text1))
            {
                spriteBatch.DrawString(m_DialogFont, m_Text1, s_DrawVector, ColorHelper.AddAlpha(Color.Black, foregroundAlpha));
                s_DrawVector.Y += m_DialogFontHeight;
            }
            if (!string.IsNullOrEmpty(m_Text2))
            {
                spriteBatch.DrawString(m_DialogFont, m_Text2, s_DrawVector, ColorHelper.AddAlpha(Color.Black, foregroundAlpha));
            }

            // Draw the continue prompt
            if (m_IsTextFullyRevealed)
            {
                s_DrawVector.X = referenceX + m_BackgroundTexture.Width - 50;
                s_DrawVector.Y = referenceY + m_BackgroundTexture.Height - 50;
                spriteBatch.Draw(m_ButtonTexture, s_DrawVector, ColorHelper.AddAlpha(Color.White, foregroundAlpha));
            }

            // End sprite batch
            spriteBatch.End();
        }

        #endregion
    }
}
