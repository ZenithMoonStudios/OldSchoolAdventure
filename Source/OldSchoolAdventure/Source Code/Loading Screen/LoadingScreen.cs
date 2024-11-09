using Destiny;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace OSA
{
    /// <summary>
	/// Loading screen class.
	/// 
	/// The loading screen initiates the transition off of all screens, and displays
	/// itself. Once all screens are off, it activates the next screen. This ensures
	/// that the menu system is entirely gone before the game itself is loaded.
    /// </summary>
    public class LoadingScreen : Screen
    {
        /// <summary>
        /// Fonts.
        /// </summary>
        private SpriteFont m_LoadingFont = null;

        /// <summary>
        /// Loading message.
        /// </summary>
        private const string c_LoadingMessage = "Loading...";

        /// <summary>
        /// Whether loading is slow.
        /// </summary>
        private bool m_LoadingIsSlow = false;

        /// <summary>
        /// Whether the other screens are deactivated.
        /// </summary>
        private bool m_OtherScreensGone = false;

        /// <summary>
        /// Screens to load.
        /// </summary>
        private Screen[] m_ScreensToLoad = null;

        /// <summary>
        /// Constructor.
        /// 
        /// This method is private, as the loading screen should be initiated
        /// through the static Load method.
        /// </summary>
        /// <param name="p_ScreenManager">Screen manager.</param>
        /// <param name="p_LoadingIsSlow">Whether loading is slow.</param>
        /// <param name="p_ScreensToLoad">Screens to load.</param>
        private LoadingScreen(ScreenManager p_ScreenManager, bool p_LoadingIsSlow, Screen[] p_ScreensToLoad)
        {
            m_LoadingIsSlow = p_LoadingIsSlow;
            m_ScreensToLoad = p_ScreensToLoad;

            this.TransitionOnTime = TimeSpan.FromSeconds(0.5);
            this.ContentRelativeFolder = "Loading Screen";
        }

        /// <summary>
        /// Load content.
        /// </summary>
        public override void LoadContent()
        {
            // Load content manager, etc.
            base.LoadContent();

            // Load resources
            m_LoadingFont = this.ContentManager.Load<SpriteFont>("Font");
        }

        /// <summary>
        /// Unload content.
        /// </summary>
        public override void UnloadContent()
        {
            // Unload resources
            m_LoadingFont = null;

            // Unload content manager, etc.
            base.UnloadContent();
        }

        /// <summary>
        /// Activate the loading screen.
        /// </summary>
		/// <param name="p_ScreenManager">Screen manager.</param>
		/// <param name="p_LoadingIsSlow">Whether loading is slow.</param>
		/// <param name="p_ScreensToLoad">Screens to load.</param>
        public static void Load(ScreenManager p_ScreenManager, bool p_LoadingIsSlow, params Screen[] p_ScreensToLoad)
        {
            // Transition off all screens
            ScreenList screens = p_ScreenManager.Screens;
            for (int i = screens.Count - 1; i >= 0; i--)
            {
                screens[i].ExitScreen();
            }

            // Create and activate the loading screen
            p_ScreenManager.AddScreen(new LoadingScreen(p_ScreenManager, p_LoadingIsSlow, p_ScreensToLoad));
        }

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

            // If screens have finished transitioning off, perform the load
            if (m_OtherScreensGone)
            {
                this.ScreenManager.RemoveScreen(this);
                foreach (Screen screen in m_ScreensToLoad)
                {
                    if (screen != null)
                    {
                        this.ScreenManager.AddScreen(screen);
                    }
                }

                // Once the load has finished, we need to tell the game that we have just
                // finished a very long frame, and that it should not try to catch up
                this.ScreenManager.Game.ResetElapsedTime();
            }
        }

        /// <summary>
        /// Draw screen.
        /// </summary>
        /// <param name="p_GameTime">Game time.</param>
        public override void Draw(GameTime p_GameTime)
        {
            // If there is only one active screen (this one), then other screens are
            // gone - this needs to be done in Draw because the transition needs to
            // have drawn at least one frame in order to look good
            if (this.State == ScreenState.Active && this.ScreenManager.Screens.Count == 1)
            {
                m_OtherScreensGone = true;
            }

            // If loading is slow, display a loading message
            if (m_LoadingIsSlow)
            {
                // Draw background
                SpriteBatch spriteBatch = this.SpriteBatch;
                this.ScreenManager.DarkenScreen(this.TransitionAlpha);

                // Start drawing sprites
                spriteBatch.Begin();

                // Draw the loading message
                Viewport viewport = this.ScreenManager.GraphicsDevice.Viewport;
                Vector2 messageSize = m_LoadingFont.MeasureString(c_LoadingMessage);
                spriteBatch.DrawString(
                    m_LoadingFont,
                    c_LoadingMessage,
                    new Vector2((viewport.Width - messageSize.X) / 2, (viewport.Height - messageSize.Y) / 2),
                    ColorHelper.AddAlpha(Color.White, this.TransitionAlpha)
                    );

                // Finish drawing sprites
                spriteBatch.End();
            }
        }
    }
}
