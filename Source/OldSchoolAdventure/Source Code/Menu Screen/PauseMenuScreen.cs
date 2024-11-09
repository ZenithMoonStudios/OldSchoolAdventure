namespace OSA
{
    /// <summary>
    /// Pause menu screen.
    /// </summary>
    class PauseMenuScreen : MenuScreen
    {
        /// <summary>
        /// Screens.
        /// </summary>
        PlayScreen m_PlayScreen;
        MenuBackgroundScreen m_BackgroundScreen;

        /// <summary>
        /// Constructor.
        /// </summary>
        public PauseMenuScreen(PlayScreen p_PlayScreen, MenuBackgroundScreen p_BackgroundScreen, bool p_CanSave) : base("Paused")
        {
            m_PlayScreen = p_PlayScreen;
            m_BackgroundScreen = p_BackgroundScreen;

            // This screen is a popup - other screens can be seen beneath it
            this.IsPopup = true;
            this.CanCancel = false;

            // Create menu entries
            MenuEntry resumeEntry = new MenuEntry("Resume", this);
            MenuEntry controlsEntry = new MenuEntry("View Controls", this);
            MenuEntry saveEntry = new MenuEntry("Save", false, true, true, false, true, p_CanSave, this, null);
            MenuEntry quitEntry = new MenuEntry("Quit", this);

            // Hook up event handlers
            resumeEntry.Selected += ResumeSelected;
            controlsEntry.Selected += ControlsSelected;
            saveEntry.Selected += SaveSelected;
            quitEntry.Selected += QuitSelected;

            // Populate menu
            this.Entries.Add(resumeEntry);
            this.Entries.Add(controlsEntry);
            this.Entries.Add(saveEntry);
            this.Entries.Add(quitEntry);

            this.EnabledGestures = Microsoft.Xna.Framework.Input.Touch.GestureType.Tap;

        }

        /// <summary>
        /// Load content.
        /// </summary>
        public override void LoadContent()
        {
            this.ScreenManager.AudioManager.IsGamePaused = true;
            base.LoadContent();
        }

        /// <summary>
        /// Unload content.
        /// </summary>
        public override void UnloadContent()
        {
            this.ScreenManager.AudioManager.IsGamePaused = false;
            base.UnloadContent();
        }

        /// <summary>
        /// Resume event handler.
        /// </summary>
        void ResumeSelected(object p_Tag)
        {
            m_BackgroundScreen.ExitScreen();
            this.ExitScreen();
        }

        /// <summary>
        /// Controls event handler.
        /// </summary>
        void ControlsSelected(object p_Tag)
        {
            this.ScreenManager.AddScreen(new ControlsScreen());
        }

        /// <summary>
        /// Save event handler.
        /// </summary>
        void SaveSelected(object p_Tag)
        {
            if (PlatformerGame.Instance.UserProfile.SavedGameState != null)
            {
                MessageBoxScreen.Show(
                    this.ScreenManager,
                    "Overwrite Save?", "Do you want to overwrite your existing saved game?",
                    "Yes", "No",
                    this.ConfirmSaveMessageBoxHandler
                    );
            }
            else
            {
                this.Save();
            }
        }

        /// <summary>
        /// Confirm save message box callback.
        /// </summary>
        void ConfirmSaveMessageBoxHandler(bool p_IsAccept)
        {
            if (p_IsAccept)
            {
                this.Save();
            }
        }

        /// <summary>
        /// Save.
        /// </summary>
        void Save()
        {
            this.ScreenManager.AddScreen(new SaveScreen());
        }

        /// <summary>
        /// Quit event handler.
        /// </summary>
        void QuitSelected(object p_Tag)
        {
            MessageBoxScreen.Show(
                this.ScreenManager,
                "Quit Game?", "Would you like to quit this game?",
                "Yes", "No",
                this.ConfirmQuitMessageBoxHandler
                );
        }

        /// <summary>
        /// Confirm quit message box callback.
        /// </summary>
        void ConfirmQuitMessageBoxHandler(bool p_IsAccept)
        {
            if (p_IsAccept)
            {
                //if (Microsoft.Phone.Shell.PhoneApplicationService.Current.State.ContainsKey("GameDeactivated")) Microsoft.Phone.Shell.PhoneApplicationService.Current.State.Remove("GameDeactivated");
                m_PlayScreen.IsQuit = true;
                this.ScreenManager.Game.Exit();
            }
        }

        /// <summary>
        /// When the user cancels the main menu, we exit the game.
        /// </summary>
        protected override void OnCancel()
        {
            MessageBoxScreen.Show(
                this.ScreenManager,
                "Exit Game?", "Are you sure you want to exit Old School Adventure?, your progress will not be saved!!",
                "Yes", "No",
                this.ConfirmQuitMessageBoxHandler
                );
        }
    }
}
