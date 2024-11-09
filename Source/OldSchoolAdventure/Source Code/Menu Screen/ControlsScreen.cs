using Destiny;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace OSA
{
    /// <summary>
    /// Controls screen.
    /// </summary>
    public class ControlsScreen : Screen
    {
        Texture2D m_BackgroundTexture;
        SpriteFont m_NormalFont;
        SpriteFont m_ButtonFont;
        SpriteFont m_ButtonTextFont;

        static Vector2 s_TempVector;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ControlsScreen()
        {
            this.TransitionOnTime = TimeSpan.FromSeconds(0.5f);
            this.TransitionOffTime = TimeSpan.FromSeconds(0.5f);
            this.ContentRelativeFolder = "Menu Screen/Controls Screen";
            this.IsPopup = true;
        }

        /// <summary>
        /// Load content.
        /// </summary>
        public override void LoadContent()
        {
            // Load content manager, etc.
            base.LoadContent();

            // Load resources
            m_BackgroundTexture = this.ContentManager.Load<Texture2D>("Background");
            m_NormalFont = this.ContentManager.Load<SpriteFont>("NormalFont");
            m_ButtonFont = this.ContentManager.Load<SpriteFont>("ButtonFont");
            m_ButtonTextFont = this.ContentManager.Load<SpriteFont>("ButtonTextFont");
        }

        #region Input

        /// <summary>
        /// Handle input. Only called when screen has focus.
        /// </summary>
        public override void HandleInput(InputState p_InputState)
        {
            base.HandleInput(p_InputState);

            if (p_InputState.IsCancel())
            {
                // Play audio
                this.AudioManager.PlayCue("MenuBack");

                this.ExitScreen();
            }
        }

        ///// <summary>
        ///// Determine whether the input state is requesting to cancel the dialog.
        ///// </summary>
        //static bool IsCancel(InputState p_InputState)
        //{
        //    return p_InputState.IsNewKeyPress(Keys.Escape) || p_InputState.IsNewKeyPress(Keys.Space) ||
        //           p_InputState.IsNewButtonPress(Buttons.A) || p_InputState.IsNewButtonPress(Buttons.B) || p_InputState.IsNewButtonPress(Buttons.Back);
        //}

        #endregion

        #region Draw

        /// <summary>
        /// Draw screen.
        /// </summary>
        /// <param name="p_GameTime">Game time.</param>
        public override void Draw(GameTime p_GameTime)
        {
            // Start drawing sprites
            SpriteBatch spriteBatch = this.SpriteBatch;
            spriteBatch.Begin();

            // Fade everything with screen transitions
            Color color = ColorHelper.AddAlpha(Color.White, this.TransitionAlpha);

            // Darken everything behind this screen
            this.ScreenManager.DarkenScreen(this.TransitionAlpha, spriteBatch);

            // Draw background
            s_TempVector.X = (this.Viewport.Width - m_BackgroundTexture.Width) / 2;
            s_TempVector.Y = (this.Viewport.Height - m_BackgroundTexture.Height) / 2;
            spriteBatch.Draw(m_BackgroundTexture, s_TempVector, color);

            // Draw text
#if !MOBILE
            s_TempVector.X = 144; s_TempVector.Y = 300;
            spriteBatch.DrawString(m_NormalFont, "Run left", s_TempVector, color);
            s_TempVector.X = 123; s_TempVector.Y = 330;
            spriteBatch.DrawString(m_NormalFont, "Run right", s_TempVector, color);
            s_TempVector.X = 500; s_TempVector.Y = 115;
            spriteBatch.DrawString(m_NormalFont, "Pause", s_TempVector, color);
            s_TempVector.X = 840; s_TempVector.Y = 100;
            spriteBatch.DrawString(m_NormalFont, "Enter door", s_TempVector, color);
            s_TempVector.X = 840; s_TempVector.Y = 130;
            spriteBatch.DrawString(m_NormalFont, "Read sign", s_TempVector, color);
            s_TempVector.X = 1005; s_TempVector.Y = 280;
            spriteBatch.DrawString(m_NormalFont, "Jump", s_TempVector, color);
            s_TempVector.X = 1005; s_TempVector.Y = 310;
            spriteBatch.DrawString(m_NormalFont, "Wall jump", s_TempVector, color);
            s_TempVector.X = 1005; s_TempVector.Y = 340;
            spriteBatch.DrawString(m_NormalFont, "Fly", s_TempVector, color);

            // Draw buttons and text
            s_TempVector.X = 140; s_TempVector.Y = 545;
            spriteBatch.DrawString(m_ButtonFont, ")", s_TempVector, color, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);
            s_TempVector.X = 205; s_TempVector.Y = 580;
            spriteBatch.DrawString(m_ButtonTextFont, "Back", s_TempVector, color);
#endif

            // Finish drawing sprites
            spriteBatch.End();
        }

        #endregion
    }
}
