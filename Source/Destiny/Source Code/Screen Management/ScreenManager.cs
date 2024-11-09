using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace Destiny
{
    /// <summary>
    /// Screen manager class.
    /// </summary>
    public class ScreenManager : DrawableGameComponent
    {
        /// <summary>
        /// Sprite batch.
        /// </summary>
        private SpriteBatch m_SpriteBatch = null;

        /// <summary>
        /// Resources.
        /// </summary>
        private Texture2D m_BlackTexture;

        /// <summary>
        /// Screens.
        /// </summary>
        public ScreenList Screens { get; private set; }

        /// <summary>
        /// Input state.
        /// </summary>
        public InputState InputState { get; private set; }

        /// <summary>
        /// Audio manager.
        /// </summary>
        public AudioManager AudioManager { get; private set; }

        /// <summary>
        /// State.
        /// </summary>
        private bool m_IsInitialized = false;
        private bool m_HasUpdatedSinceLastDraw = true;

        /// <summary>
        /// Screens to update.
        /// </summary>
        private ScreenStack m_ScreensToUpdate = new ScreenStack();

        /// <summary>
        /// Temporary objects.
        /// </summary>
        private Rectangle m_TempRectangle;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="p_Game">Game.</param>
        public ScreenManager(Game p_Game, AudioManager p_AudioManager) : base(p_Game)
        {
            this.InputState = new InputState();
            this.AudioManager = p_AudioManager;

            this.Screens = new ScreenList();

            // we must set EnabledGestures before we can query for them, but
            // we don't assume the game wants to read them.
            TouchPanel.EnabledGestures = GestureType.None;
        }

        /// <summary>
        /// Sprite batch.
        /// </summary>
        public SpriteBatch SpriteBatch { get { return m_SpriteBatch; } }

        /// <summary>
        /// Initialize.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            m_IsInitialized = true;
        }

        /// <summary>
        /// Load content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create sprite batch
            m_SpriteBatch = new SpriteBatch(this.GraphicsDevice);

            // Load black texture for darkening screen
            m_BlackTexture = TextureHelper.CreateUnitTexture(Color.Black, this.GraphicsDevice);

            // Load content for all screens
            for (int i = 0; i < this.Screens.Count; i++)
            {
                this.Screens[i].LoadContent();
            }
        }

        /// <summary>
        /// Unload content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Unload screens
            for (int i = 0; i < this.Screens.Count; i++)
            {
                this.Screens[i].UnloadContent();
            }

            // Unload resources
            if (m_BlackTexture != null)
            {
                m_BlackTexture.Dispose();
                m_BlackTexture = null;
            }
        }

        /// <summary>
        /// Update screens.
        /// </summary>
        /// <param name="p_GameTime">Game time.</param>
        public override void Update(GameTime p_GameTime)
        {
            this.DoUpdate(p_GameTime);
        }

        void DoUpdate(GameTime p_GameTime)
        {
            // Only update if the guide is not visible
            bool isGameActive = Game.IsActive;
            this.AudioManager.IsGameActive = isGameActive;
            if (isGameActive)
            {
                // Update input and audio
                this.InputState.Update();

                // Prepare a list of screens to update
                m_ScreensToUpdate.Clear();
                for (int i = 0; i < this.Screens.Count; i++)
                {
                    m_ScreensToUpdate.Push(this.Screens[i]);
                }

                // Topmost screen has focus if the game is active
                bool hasFocus = this.Game.IsActive;

                // Topmost screen is never covered by another screen
                bool isCoveredByOtherScreen = false;

                // Process screens, starting from the top
                while (m_ScreensToUpdate.Count > 0)
                {
                    // Retrieve screen
                    Screen screen = m_ScreensToUpdate.Pop();

                    // Screen state can change during update, so store it upfront to ensure that only
                    // 1 screen ever gets focus in a single frame
                    ScreenState screenState = screen.State;

                    if (screenState == ScreenState.TransitionOn || screenState == ScreenState.Active)
                    {
                        // If screen has focus, handle input
                        if (hasFocus)
                        {
                            screen.HandleInput(this.InputState);
                        }
                    }

                    // Update screen
                    screen.Update(p_GameTime, screen.CanHaveFocus && hasFocus, isCoveredByOtherScreen);

                    if (screenState == ScreenState.TransitionOn || screenState == ScreenState.Active)
                    {
                        // If screen has focus, it has already handled input so let it go
                        if (hasFocus && screen.CanHaveFocus)
                        {
                            // Set focus to false for the next screen
                            hasFocus = false;
                        }

                        // If screen is not a pop-up, it covers the next screen
                        if (!screen.IsPopup)
                        {
                            isCoveredByOtherScreen = true;
                        }
                    }
                }
            }
            m_HasUpdatedSinceLastDraw = true;
        }

        /// <summary>
        /// Draw screens.
        /// </summary>
        /// <param name="p_GameTime">Game time.</param>
        public override void Draw(GameTime p_GameTime)
        {
            if (m_HasUpdatedSinceLastDraw)
            {
                // Draw screens
                for (int i = 0; i < this.Screens.Count; i++)
                {
                    Screen screen = this.Screens[i];
                    if (screen.State != ScreenState.Hidden && screen.ContentManager != null)
                    {
                        screen.Draw(p_GameTime);
                    }
                }
                m_HasUpdatedSinceLastDraw = false;
            }
        }

        /// <summary>
        /// Adds a new screen.
        /// </summary>
        /// <param name="p_Screen">Screen to add.</param>
        public void AddScreen(Screen p_Screen)
        {
            this.AddScreen(this.Screens.Count, p_Screen);

        }

        /// <summary>
        /// Adds a new screen.
        /// </summary>
        /// <param name="p_ScreenInded">Screen index.</param>
        /// <param name="p_Screen">Screen to add.</param>
        public void AddScreen(int p_ScreenIndex, Screen p_Screen)
        {

            p_Screen.ScreenManager = this;
            p_Screen.IsExiting = false;
            if (m_IsInitialized)
            {
                p_Screen.LoadContent();
            }
            if (p_ScreenIndex > Screens.Count) p_ScreenIndex = Screens.Count;
            this.Screens.Insert(p_ScreenIndex, p_Screen);

            // update the TouchPanel to respond to gestures this screen is interested in
            TouchPanel.EnabledGestures = p_Screen.EnabledGestures;
        }

        /// <summary>
        /// Removes a screen.
        /// 
        /// Screen.ExitScreen should normally be called to do this, so that
        /// the screen can gradually transition off rather just being
        /// instantly removed.
        /// </summary>
        /// <param name="p_Screen">Screen to remove.</param>
        public void RemoveScreen(Screen p_Screen)
        {
            if (m_IsInitialized && p_Screen.ContentManager != null)
            {
                p_Screen.UnloadContent();
            }
            this.Screens.Remove(p_Screen);

            // if there is a screen still in the manager, update TouchPanel
            // to respond to gestures that screen is interested in.
            if (this.Screens.Count > 0)
            {
                TouchPanel.EnabledGestures = this.Screens[this.Screens.Count - 1].EnabledGestures;
            }
        }

        /// <summary>
        /// Darken the entire viewport.
        /// </summary>
        /// <param name="p_Alpha">The level of darkness.</param>
        public void DarkenScreen(byte p_Alpha)
        {
            m_SpriteBatch.Begin();
            this.DarkenScreen(p_Alpha, m_SpriteBatch);
            m_SpriteBatch.End();
        }

        /// <summary>
        /// Darken the entire viewport.
        /// </summary>
        /// <param name="p_Alpha">The level of darkness.</param>
        public void DarkenScreen(byte p_Alpha, SpriteBatch p_SpriteBatch)
        {
            Viewport viewport = this.GraphicsDevice.Viewport;
            m_TempRectangle.Width = viewport.Width;
            m_TempRectangle.Height = viewport.Height;
            p_SpriteBatch.Draw(m_BlackTexture, m_TempRectangle, new Color((byte)0, (byte)0, (byte)0, p_Alpha));
        }
    }
}
