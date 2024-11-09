using Destiny;

namespace OSA
{
    /// <summary>
    /// Play screen class.
    /// </summary>
    public abstract class PlayScreen : Screen
    {
        public bool IsQuit { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public PlayScreen()
        {
            this.IsQuit = false;
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

            // Handle pausing and quiting
            if (this.IsQuit)
            {
                LoadingScreen.Load(
                    this.ScreenManager,
                    false,
                    new MenuBackgroundScreen(MenuBackgroundScreen.Modes.MainMenu),
                    new StartScreen()
                    );
            }
            else if (!p_InputState.IsActivePlayerConnected || p_InputState.IsPause())
            {
                MenuBackgroundScreen backgroundScreen = new MenuBackgroundScreen(MenuBackgroundScreen.Modes.PauseMenu);
                this.ScreenManager.AddScreen(backgroundScreen);
                this.ScreenManager.AddScreen(new PauseMenuScreen(this, backgroundScreen, CanSaveFromScreen(this)));
            }
        }

        private static bool CanSaveFromScreen(Screen p_Screen)
        {
            return p_Screen is LevelPlayScreen || p_Screen is PlayConversationScreen;
        }

        //private static bool IsPause(InputState p_InputState)
        //{
        //    return p_InputState.IsNewKeyPress(Keys.Escape) || p_InputState.IsNewButtonPress(Buttons.Start);
        //}

        /// <summary>
        /// When the user cancels the main menu, we exit the game.
        /// </summary>
        protected override void OnCancel()
        {

            //Need better Handler
            //ScreenManager.Game.Exit();
        }
        #endregion
    }
}
