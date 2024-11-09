using Destiny;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace OSA
{
    /// <summary>
	/// Save screen class.
    /// </summary>
    public class SaveScreen : Screen
    {
        /// <summary>
        /// Resources.
        /// </summary>
        private SpriteFont m_Font = null;

        /// <summary>
        /// Save message.
        /// </summary>
        private const string c_Message = "Saving...";

        /// <summary>
        /// Constructor.
        /// </summary>
        public SaveScreen()
        {
            // This screen is a popup - other screens can be seen beneath it
            this.IsPopup = true;

            this.TransitionOnTime = TimeSpan.FromSeconds(0.5);
            this.TransitionOffTime = TimeSpan.FromSeconds(0.5);
            this.ContentRelativeFolder = "Save Screen";
        }

        /// <summary>
        /// Load content.
        /// </summary>
        public override void LoadContent()
        {
            // Load content manager, etc.
            base.LoadContent();

            // Load resources
            m_Font = this.ContentManager.Load<SpriteFont>("Font");
        }

        /// <summary>
        /// Unload content.
        /// </summary>
        public override void UnloadContent()
        {
            // Unload resources
            m_Font = null;

            // Unload content manager, etc.
            base.UnloadContent();
        }

        private bool m_HasDrawn = false;
        private bool m_HasSaved = false;

        /// <summary>
        /// Update screen.
        /// Called regardless of screen state.
        /// </summary>
        /// <param name="p_GameTime">Game time.</param>
        /// <param name="p_HasFocus">Whether screen has focus.</param>
        /// <param name="p_IsCoveredByOtherScreen">Whether screen is covered by another screen.</param>
        public override void Update(GameTime p_GameTime, bool p_HasFocus, bool p_IsCoveredByOtherScreen)
        {
            base.Update(p_GameTime, p_HasFocus, p_IsCoveredByOtherScreen);

            if (p_HasFocus && m_HasDrawn && !m_HasSaved)
            {
                m_HasSaved = true;
                bool isSaved = PlatformerGame.Instance.SaveGame();
                if (!isSaved)
                {
                    MessageBoxScreen.Show(
                        this.ScreenManager,
                        "Save Failed",
                        "Failed to save your game.\n\nThere may be a problem with your storage device, or it may be full.",
                        "OK", null,
                        (isAccept) => this.ExitScreen()
                        );
                }
                else
                {
                    this.ExitScreen();
                }
            }
        }

        /// <summary>
        /// Draw screen.
        /// </summary>
        public override void Draw(GameTime p_GameTime)
        {
            // Draw background
            SpriteBatch spriteBatch = this.SpriteBatch;
            this.ScreenManager.DarkenScreen(this.TransitionAlpha);

            // Start drawing sprites
            spriteBatch.Begin();

            // Draw the message
            Viewport viewport = this.ScreenManager.GraphicsDevice.Viewport;
            Vector2 messageSize = m_Font.MeasureString(c_Message);
            spriteBatch.DrawString(
                m_Font,
                c_Message,
                new Vector2((viewport.Width - messageSize.X) / 2, (viewport.Height - messageSize.Y) / 2),
                ColorHelper.AddAlpha(Color.White, this.TransitionAlpha)
                );

            // Finish drawing sprites
            spriteBatch.End();

            if (this.TransitionAlpha == 255)
            {
                m_HasDrawn = true;
            }
        }
    }
}
