using Destiny;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace OSA
{
    /// <summary>
    /// Other games screen.
    /// </summary>
    public class OtherGamesScreen : Screen
    {
        /// <summary>
        /// Resources.
        /// </summary>
        Texture2D m_BackgroundTexture;
        SpriteFont m_ButtonFont;
        SpriteFont m_ButtonTextFont;

        /// <summary>
        /// Temporary objects.
        /// </summary>
        static Vector2 s_TempVector;

        /// <summary>
        /// Constructor.
        /// </summary>
        public OtherGamesScreen()
        {
            this.TransitionOnTime = TimeSpan.FromSeconds(0.5f);
            this.TransitionOffTime = TimeSpan.FromSeconds(0.5f);
            this.ContentRelativeFolder = "Menu Screen/Other Games Screen";
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
        //private static bool IsCancel(InputState p_InputState)
        //{
        //    return p_InputState.IsNewKeyPress(Keys.Escape) ||
        //           p_InputState.IsNewButtonPress(Buttons.B) || p_InputState.IsNewButtonPress(Buttons.Back);
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
            spriteBatch.Draw(m_BackgroundTexture, Vector2.Zero, color);

            // Draw buttons and text
            s_TempVector.X = 140; s_TempVector.Y = 545;
            spriteBatch.DrawString(m_ButtonFont, ")", s_TempVector, color, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);
            s_TempVector.X = 205; s_TempVector.Y = 580;
            spriteBatch.DrawString(m_ButtonTextFont, "Back", s_TempVector, color);

            // Finish drawing sprites
            spriteBatch.End();
        }

        #endregion
    }
}
