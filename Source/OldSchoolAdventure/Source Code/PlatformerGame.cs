using Destiny;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
#if(MOBILE)
using System.Windows.Threading;
using Microsoft.Phone.Info;
using System.Diagnostics;
using Microsoft.Phone;
#endif

namespace OSA
{
    /// <summary>
    /// Platformer game class.
    /// </summary>
    public class PlatformerGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager m_GraphicsDeviceManager;
        public ScreenManager ScreenManager { get; private set; }
        public AudioManager AudioManager { get; private set; }
        public PurchasingGameComponent PurchasingComponent { get; private set; }

        /// <summary>
        /// Game settings and data.
        /// </summary>
        public LevelPlayScreen PlayScreen { get; set; }
        public PlatformerGameSettings GameSettings { get; private set; }
        public PlatformerGameData GameData { get; private set; }
        public UserProfile UserProfile { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public PlatformerGame()
        {
            // Set content root directory
            this.Content.RootDirectory = "Content";

            // Create and initialize graphics device manager
            m_GraphicsDeviceManager = new GraphicsDeviceManager(this);

#if MOBILE
            m_GraphicsDeviceManager.PreferredBackBufferWidth = 800;
			m_GraphicsDeviceManager.PreferredBackBufferHeight = 480;
			m_GraphicsDeviceManager.IsFullScreen = true;
#else
            m_GraphicsDeviceManager.PreferredBackBufferWidth = 1280;
            m_GraphicsDeviceManager.PreferredBackBufferHeight = 720;
            this.Window.Title = "Old School Adventure";
#endif

            // Create audio manager
            this.AudioManager = new AudioManager(this.Content);

            // Create and initialize screen manager
            this.ScreenManager = new ScreenManager(this, this.AudioManager);

#if (!MOBILE)
            // Add splash screen
            this.ScreenManager.AddScreen(new SplashScreen("Background", delegate ()
            {
                LoadingScreen.Load(this.ScreenManager, false, new SplashScreen("Background2", delegate ()
                {
                    LoadingScreen.Load(
                        this.ScreenManager,
                        false,
                        new MenuBackgroundScreen(MenuBackgroundScreen.Modes.MainMenu),
                        new StartScreen()
                        );
                }
                    ));
            }
            ));
#endif
            this.Components.Add(this.ScreenManager);
            this.Components.Add(this.PurchasingComponent = new PurchasingGameComponent(this));
#if XBOX
			this.Components.Add(new GamerServicesComponent(this));
#endif
#if SIMULATE_TRIAL_MODE
            Guide.SimulateTrialMode = true;
#endif

            // Load game settings
            GameSettings = GameSettings.LoadSettings(Content.Load<Destiny.GameSettings>("GameSettings"));


#if SHOW_OVERSCAN
			// Register unsafe area component
			UnsafeAreaOverlayComponent unsafeAreaComponent = new UnsafeAreaOverlayComponent(this);
//			unsafeAreaComponent.NoActionAreaColor = Color.Black;
//			unsafeAreaComponent.UnsafeAreaColor = Color.Black;
			this.Components.Add(unsafeAreaComponent);
#endif

            // Lock in the frame rate (30fps for phone, 60fps for others
#if MOBILE
            this.TargetElapsedTime = TimeSpan.FromTicks(((long)(10000000.0f / 30.0f + 0.5f)));
#else
            this.TargetElapsedTime = TimeSpan.FromTicks(((long)(10000000.0f / 60.0f + 0.5f)));
#endif

            // Register console commands
            this.RegisterCommands();

#if (DEBUG && MOBILE)
			var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
			timer.Tick += (s, e) =>
			{
				var memuse = (long)DeviceExtendedProperties.GetValue("ApplicationPeakMemoryUsage");
				var maxmem = (long)DeviceExtendedProperties.GetValue("DeviceTotalMemory");
				memuse /= 1024 * 1024;
				maxmem /= 1024 * 1024;
				Debug.WriteLine(String.Format("Mem usage: {0} / {1} MB", memuse, maxmem));
			};
			timer.Start();
#endif

#if (MOBILE)
			Instance = this;
#endif

        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Draw the game.
        /// </summary>
        /// <param name="p_GameTime">Game time.</param>
        protected override void Draw(GameTime p_GameTime)
        {
            // Clear the screen
            m_GraphicsDeviceManager.GraphicsDevice.Clear(Color.Black);

            // Base class handles everything else
            base.Draw(p_GameTime);
        }

        #region Command handlers

        /// <summary>
        /// Register commands.
        /// </summary>
        protected void RegisterCommands()
        {
            CommandManager.Instance.RegisterCommand("restart", this.RestartGame);
            CommandManager.Instance.RegisterCommand("modes", this.ConsoleListModes);
            CommandManager.Instance.RegisterCommand("mode", this.ConsoleSetMode);
        }

        /// <summary>
        /// List game modes command handler.
        /// </summary>
        public void ConsoleListModes(string[] p_Arguments, ref CommandResult p_CommandResult)
        {
            for (int i = 0; i < this.GameSettings.Modes.Count; i++)
            {
                p_CommandResult.AddOutputLine(string.Format(
                    "{0}: {1}",
                    i + 1,
                    this.GameSettings.Modes[i].Name
                    ));
            }
        }

        /// <summary>
        /// Set mode command handler.
        /// </summary>
        public void ConsoleSetMode(string[] p_Arguments, ref CommandResult p_CommandResult)
        {
            if (p_Arguments == null || p_Arguments.Length == 0)
            {
                p_CommandResult.AddOutputLine("Usage: 'mode <mode_number>'");
            }
            else
            {
                int modeNumber = 0;
                IntHelper.TryParse(p_Arguments[0], out modeNumber);
                if (modeNumber == 0 || modeNumber > this.GameSettings.Modes.Count)
                {
                    p_CommandResult.AddOutputLine(string.Format("'{0}' is not a valid mode number", p_Arguments[0]));
                }
                else
                {
                    this.SetMode(this.GameSettings.Modes[modeNumber - 1].Name);
                }
            }
        }

        public void SetMode(string p_Name)
        {
            CollectibleStore store = this.GameData.Store;
            for (int i = 0; i < this.GameSettings.Modes.Count; i++)
            {
                PlatformerGameSettings.GameMode mode = this.GameSettings.Modes[i];
                if (p_Name == mode.Name)
                {
                    // Add items to store
                    foreach (PlatformerGameSettings.GameItem item in mode.Items)
                    {
                        if (!store.Has(item.Path))
                        {
                            store.Collect(item.Path, 1);
                        }
                    }
                }
                else
                {
                    // Remove items from store
                    foreach (PlatformerGameSettings.GameItem item in mode.Items)
                    {
                        if (store.Has(item.Path))
                        {
                            store.Use(item.Path, store.Quantity(item.Path));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Restart game command handler.
        /// </summary>
        /// <param name="p_Arguments">Arguments.</param>
        /// <param name="p_CommandResult">Result.</param>
        public void RestartGame(string[] p_Arguments, ref CommandResult p_CommandResult)
        {
            if (p_Arguments == null || p_Arguments.Length == 0)
            {
                p_CommandResult.AddOutputLine("Usage: 'restart <mode_number>'");
            }
            else
            {
                int modeNumber = 0;
                IntHelper.TryParse(p_Arguments[0], out modeNumber);
                if (modeNumber == 0 || modeNumber > this.GameSettings.Modes.Count)
                {
                    p_CommandResult.AddOutputLine(string.Format("'{0}' is not a valid mode number", p_Arguments[0]));
                }
                else
                {
                    this.NewGame(this.GameSettings.Modes[modeNumber - 1]);
                }
            }
        }

        #endregion

        #region Load and save

        /// <summary>
        /// Save game.
        /// </summary>
        public bool SaveGame()
        {
            return SaveGame(null);
        }

        /// <summary>
        /// Save game.
        /// </summary>
        public bool SaveGame(String FileName)
        {
            GameState gameState = new GameState();
            MainCharacter hero = this.PlayScreen.MainCharacter;
            gameState.ActiveFrames = this.GameData.ActiveFrames + this.PlayScreen.ActiveFrames;
            gameState.Level = this.GetLevelIndex(this.PlayScreen.Level.Name);
            gameState.Collectibles = hero.Store;
            gameState.LastDoor = hero.LastDoor;
            return this.UserProfile.SaveGameState(gameState, FileName);
        }

        #endregion

        #region Start game and load level

        /// <summary>
        /// Start a new game.
        /// </summary>
        public void NewGame(PlatformerGameSettings.GameMode p_Mode)
        {
            // Clear player data
            this.GameData = new PlatformerGameData();

            // Populate store
            foreach (PlatformerGameSettings.GameItem item in p_Mode.Items)
            {
                this.GameData.Store.Collect(item.Path, 1);
            }

            // Load the firt level
            this.LoadNextLevel(null);
        }

        protected override void OnActivated(object sender, EventArgs args)
        {
            base.OnActivated(sender, args);
            return;
            /*
            // Add splash screen
            this.ScreenManager.AddScreen(new SplashScreen("Background", delegate ()
            {
                LoadingScreen.Load(this.ScreenManager, false, new SplashScreen("Background2", delegate ()
                {
                    LoadingScreen.Load(
                        this.ScreenManager,
                        false,
                        new MenuBackgroundScreen(MenuBackgroundScreen.Modes.MainMenu),
                        new StartScreen()
                        );
                }
                    ));
            }
            ));
            */
        }

        protected override void OnDeactivated(object sender, EventArgs args)
        {
            base.OnDeactivated(sender, args);
            if (this.PlayScreen?.MainCharacter != null)
            {
                bool state = SaveGame("SaveState");
            }
        }

        /// <summary>
        /// Load an existing game.
        /// </summary>
        public void LoadGame()
        {
            // Clear player data
            GameState gameState = this.UserProfile.SavedGameState;
            this.GameData = new PlatformerGameData(gameState.ActiveFrames, gameState.Collectibles);

            // Load the specified level
            this.LoadLevel(this.GameSettings.Levels[gameState.Level].Name, gameState.LastDoor);
        }

        public void TrialCheckThenLoadNextLevel(string p_CurrentLevelName)
        {
            if (GuideHelper.IsTrialMode)
            {
                PlatformerGame.Instance.SaveGame();
                LoadingScreen.Load(this.ScreenManager, false, new SalesScreen());
            }
            else
            {
                this.LoadNextLevel(p_CurrentLevelName);
            }
        }

        /// <summary>
        /// Load next level.
        /// </summary>
        public Level LoadNextLevel(string p_CurrentLevelName)
        {
            Level result = null;
            if (string.IsNullOrEmpty(p_CurrentLevelName))
            {
                result = this.LoadLevel(this.GameSettings.Levels[0].Name);
            }
            else
            {
                int numberOfLevels = this.GameSettings.Levels.Count;
                int currentLevelIndex = GetLevelIndex(p_CurrentLevelName);
                if (currentLevelIndex + 1 < numberOfLevels)
                {
                    string levelName = this.GameSettings.Levels[currentLevelIndex + 1].Name;
                    result = this.LoadLevel(levelName);
                }
                else
                {
                    LoadingScreen.Load(this.ScreenManager, false, new CreditsScreen(true));
                }
            }
            return result;
        }

        int GetLevelIndex(string p_LevelName)
        {
            int result = 0;
            PlatformerGameSettings.GameLevelList levels = this.GameSettings.Levels;
            for (int i = 0; i < levels.Count; i++)
            {
                if (levels[i].Name == p_LevelName)
                {
                    result = i;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Load a level.
        /// </summary>
        public Level LoadLevel(string p_Name)
        {
            return this.LoadLevel(p_Name, null);
        }

        /// <summary>
        /// Load a level.
        /// </summary>
        public Level LoadLevel(string p_Name, DoorTarget p_LastDoor)
        {
            Level level = null;
            //try
            //{
            level = LevelFactory.Load(p_Name, Path.Combine("Levels", p_Name), this.Content);
            if (level != null)
            {
                if (p_LastDoor == null && level.IntroPages != null && level.IntroPages.Count > 0)
                {
                    LoadingScreen.Load(
                        this.ScreenManager,
                        true,
                        new LevelIntroBackgroundScreen(level),
                        new LevelIntroScreen(level)
                        );
                }
                else
                {
                    LoadingScreen.Load(
                        this.ScreenManager,
                        true,
                        new LevelPlayScreen(level, p_LastDoor)
                        );
                }
            }
            //}
            //catch
            //{
            //    // Return to main menu
            //    LoadingScreen.Load(
            //        this.ScreenManager,
            //        false,
            //        new MenuBackgroundScreen(MenuBackgroundScreen.Modes.MainMenu),
            //        new StartScreen()
            //        );
            //}
            return level;
        }

        #endregion

        #region Singleton

        private static PlatformerGame s_Instance = null;
        public static PlatformerGame Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = new PlatformerGame();
                }
                return s_Instance;
            }
            private set
            {
                s_Instance = value;
            }
        }

        #endregion
    }
}
