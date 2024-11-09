using Destiny;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace OSA
{
    /// <summary>
    /// Play screen class.
    /// </summary>
    public class LevelPlayScreen : PlayScreen
    {
        /// <summary>
        /// Timer.
        /// </summary>
        int m_PreviousLevelFrames;

        /// <summary>
        /// Cache.
        /// </summary>
        Dictionary<string, Room> m_PathToRoom = new Dictionary<string, Room>();
        Dictionary<string, TileSet> m_PathToTileSet = new Dictionary<string, TileSet>();
        Dictionary<string, TileTemplate> m_PathToTileTemplate = new Dictionary<string, TileTemplate>();
        Dictionary<string, TerrainTemplate> m_PathToTerrainTemplate = new Dictionary<string, TerrainTemplate>();
        public RoomObjectTemplateManager RoomObjectTemplateManager { get; private set; }

        List<Room> m_Rooms = new List<Room>();

        /// <summary>
        /// Scene objects.
        /// </summary>
        public Level Level { get; private set; }
        public Room Room { get; private set; }
        public MainCharacter MainCharacter { get; private set; }

        /// <summary>
        /// Drawing.
        /// </summary>
        public Camera Camera { get; set; }
        public DrawManager DrawManager { get; private set; }

        /// <summary>
        /// Status.
        /// </summary>
        private DoorTarget m_RequestedTarget = new DoorTarget();

        /// <summary>
        /// Constructor.
        /// </summary>
        public LevelPlayScreen(Level p_Level) : this(p_Level, null) { }
        public LevelPlayScreen(Level p_Level, DoorTarget p_LastDoor)
        {
            this.IsQuit = false;

            this.Level = p_Level;
            this.RequestNavigation(p_LastDoor);
            this.ContentRelativeFolder = "Level Play Screen";

            this.EnabledGestures = Microsoft.Xna.Framework.Input.Touch.GestureType.Hold |
                Microsoft.Xna.Framework.Input.Touch.GestureType.Flick |
                Microsoft.Xna.Framework.Input.Touch.GestureType.Tap |
                Microsoft.Xna.Framework.Input.Touch.GestureType.DoubleTap;

        }

        #region Load and Unload content

        /// <summary>
        /// Load content.
        /// </summary>
        public override void LoadContent()
        {
            // Load content manager, etc.
            base.LoadContent();

            // Load caches
            this.RoomObjectTemplateManager = new RoomObjectTemplateManager(this.ContentManager);

            // Initialize drawing
            this.DrawManager = new DrawManager(this, string.Empty);
            this.Camera = new Camera(
                new Rectangle(0, 0, this.Viewport.Width, this.Viewport.Height),
                0,
                this.GetCameraFollowObjectRegion,
                this.GetRoom
                );

            // Initialize timer
            m_PreviousLevelFrames = PlatformerGame.Instance.GameData.ActiveFrames;

            // Initialize main character for this level
            this.MainCharacter = PlatformerGame.Instance.GameData.Player;
            this.MainCharacter.Initialize(this);
            this.MainCharacter.LoadContentAndChildren(this.DrawManager);

            // Create status bar
            //m_StatusBar = new PlayStatusScreen(this, this.MainCharacter);
            //this.ScreenManager.AddScreen(m_StatusBar);

            // Load level rooms
            foreach (string roomPath in this.Level.Rooms)
            {
                this.LoadRoom(roomPath);
            }

            // Navigate to the first door
            if (m_RequestedTarget.IsEmpty)
            {
                this.RequestNavigation(this.Level.StartDoor);
            }
            this.HandleNavigation();

            // Register console commands
            this.RegisterCommands();

            // Register as the current play screen
            PlatformerGame.Instance.PlayScreen = this;

            // Once the load has finished, we need to tell the game that we have just
            // finished a very long frame, and that it should not try to catch up
            this.ScreenManager.Game.ResetElapsedTime();
        }

        /// <summary>
        /// Load a room.
        /// </summary>
        void LoadRoom(string p_RoomPath)
        {
            Room room = RoomFactory.Load(
                p_RoomPath,
                this.ContentManager,
                this.GetTileSet,
                this.GetTerrainTemplate,
                this.RoomObjectTemplateManager
                );
            room.LoadContentAndChildren(this.DrawManager);
            room.Screen = this;
            m_Rooms.Add(room);
            m_PathToRoom.Add(p_RoomPath, room);
        }

        /// <summary>
        /// Get a tile template.
        /// </summary>
        /// <param name="p_Path">Path to the tile template.</param>
        /// <returns>Tile template.</returns>
        private TileTemplate GetTileTemplate(string p_Path)
        {
            if (!m_PathToTileTemplate.ContainsKey(p_Path))
            {
                TileTemplate template = TileTemplateFactory.Load(p_Path, this.ContentManager);
                m_PathToTileTemplate.Add(p_Path, template);
            }
            return m_PathToTileTemplate[p_Path];
        }

        /// <summary>
        /// Get a tile set.
        /// </summary>
        /// <param name="p_Path">Path to the tile set.</param>
        /// <returns>Tile set.</returns>
        TileSet GetTileSet(string p_Path)
        {
            if (!m_PathToTileSet.ContainsKey(p_Path))
            {
                TileSet tileSet = TileSetFactory.Load(p_Path, this.ContentManager, this.GetTileTemplate);
                m_PathToTileSet.Add(p_Path, tileSet);
            }
            return m_PathToTileSet[p_Path];
        }

        /// <summary>
        /// Get a terrain template.
        /// </summary>
        /// <param name="p_Path">Path to the terrain template.</param>
        /// <returns>Terrain template.</returns>
        TerrainTemplate GetTerrainTemplate(string p_Path)
        {
            if (!m_PathToTerrainTemplate.ContainsKey(p_Path))
            {
                TerrainTemplate template = TerrainTemplateFactory.Load(p_Path, this.ContentManager);
                m_PathToTerrainTemplate.Add(p_Path, template);
            }
            return m_PathToTerrainTemplate[p_Path];
        }

        /// <summary>
        /// Unload content.
        /// </summary>
        public override void UnloadContent()
        {
            // Unload rooms
            for (int roomIndex = 0; roomIndex < m_Rooms.Count; roomIndex++)
            {
                m_Rooms[roomIndex].UnloadContentAndChildren();
            }
            m_Rooms.Clear();
            this.Room = null;

            // Unload characters
            this.MainCharacter.UnloadContentAndChildren();
            this.MainCharacter = null;

            // DeInitialize drawing
            this.Camera = null;
            this.DrawManager = null;

            // Clear cache
            m_PathToRoom.Clear();
            m_PathToTileSet.Clear();
            m_PathToTileTemplate.Clear();
            m_PathToTerrainTemplate.Clear();

            // Unload content manager, etc.
            base.UnloadContent();
        }

        #endregion

        #region Camera delegates

        private Rectangle GetCameraFollowObjectRegion()
        {
            return new Rectangle(
                (int)this.MainCharacter.Left,
                (int)this.MainCharacter.Top,
                (int)this.MainCharacter.Width,
                (int)this.MainCharacter.Height
                );
        }

        private Room GetRoom()
        {
            return this.Room;
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

            // Process player controls
            if (this.Room != null)
            {
                this.Room.HandleInputAndChildren(this.InputState);
            }
        }

        //static bool IsShowCommandConsole(InputState p_InputState) { return p_InputState.IsOpenCommandConsole(); }
        //static bool IsOpenTileEditor(InputState p_InputState) { return p_InputState.IsNewKeyPress(Keys.F2); }
        //static bool IsOpenCollectibleEditor(InputState p_InputState) { return p_InputState.IsNewKeyPress(Keys.F3); }
        //static bool IsOpenEnemyEditor(InputState p_InputState) { return p_InputState.IsNewKeyPress(Keys.F4); }
        //static bool IsOpenDirectorEditor(InputState p_InputState) { return p_InputState.IsNewKeyPress(Keys.F5); }
        //static bool IsOpenObstacleEditor(InputState p_InputState) { return p_InputState.IsNewKeyPress(Keys.F6); }
        //static bool IsOpenSpawnPointEditor(InputState p_InputState) { return p_InputState.IsNewKeyPress(Keys.F7); }
        //static bool IsOpenDoorEditor(InputState p_InputState) { return p_InputState.IsNewKeyPress(Keys.F8); }
        //static bool IsOpenSpeakerEditor(InputState p_InputState) { return p_InputState.IsNewKeyPress(Keys.F9); }
        //static bool IsOpenMusicPointEditor(InputState p_InputState) { return p_InputState.IsNewKeyPress(Keys.F10); }
        //static bool IsOpenSwitchEditor(InputState p_InputState) { return p_InputState.IsNewKeyPress(Keys.F11); }

        static bool IsPause(InputState p_InputState) { return p_InputState.IsPause(); }

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

            // If this screen has focus, update as normal
            if (p_HasFocus)
            {
                // Update room
                if (this.Room != null)
                {
                    this.Room.UpdateAndChildren();
                    this.Room.EvaluateAndHandleCollisionAndChildren(this.MainCharacter);
                    for (int i = 0; i < this.Room.Directors.Count; i++)
                    {
                        this.Room.EvaluateAndHandleCollisionAndChildren(this.Room.Directors[i]);
                    }
                    this.Room.UpdateActionStateAndChildren();
                }

                // Update timer
                int totalFrames = m_PreviousLevelFrames + this.ActiveFrames;
            }

            // Handle navigation even if screen doesn't have focus, so that the command console
            // can navigate between pages
            if (this.Room != null)
            {
                this.HandleNavigation();
                if (this.Camera != null)
                {
                    this.Camera.Update();
                }
            }
        }

        #endregion

        #region Draw

        Vector3 m_DrawTranslationVector = Vector3.Zero;

        /// <summary>
        /// Draw screen.
        /// </summary>
        /// <param name="p_GameTime">Game time.</param>
        public override void Draw(GameTime p_GameTime)
        {
            if (this.Room == null) { return; }

            this.SpriteBatch.Begin();
            this.Room.DrawAndChildren(this.DrawManager);
            this.SpriteBatch.End();

            int frameCount = this.MainCharacter.FramesSinceLastDeath;
            int framesUntilNextDeath = 4 * this.MainCharacter.FramesUntilNextDeath - 45;
            if (this.MainCharacter.FramesUntilNextDeath > 0 && framesUntilNextDeath <= 0)
            {
                framesUntilNextDeath = 1;
            }
            if (framesUntilNextDeath < frameCount && framesUntilNextDeath > 0)
            {
                frameCount = framesUntilNextDeath;
            }
            byte alpha = (byte)(255 - (255 * (Math.Min(frameCount, 60) / 60f)));
            if (alpha > 0)
            {
                this.ScreenManager.DarkenScreen(alpha);
            }
        }

        #endregion

        #region Navigation

        /// <summary>
        /// Submit a request to navigate to the specified target location.
        /// </summary>
        public void RequestNavigation(DoorTarget p_Target)
        {
            m_RequestedTarget.Set(p_Target);
        }

        /// <summary>
        /// Submit a request to navigate to the specified target location.
        /// </summary>
        public void RequestNavigation(string p_RoomPath, string p_DoorName)
        {
            m_RequestedTarget.Set(p_RoomPath, p_DoorName);
        }

        /// <summary>
        /// Handle navigation.
        /// </summary>
        private void HandleNavigation()
        {
            if (!m_RequestedTarget.IsEmpty)
            {
                // Handle end of level
                if (m_RequestedTarget.RoomPath == this.Level.EndDoor.RoomPath &&
                    m_RequestedTarget.ObjectName == this.Level.EndDoor.ObjectName
                    )
                {
                    PlatformerGame game = PlatformerGame.Instance;

                    // Save level data
                    game.GameData.SubmitLevel(this.ActiveFrames);

                    // Load next level
                    game.TrialCheckThenLoadNextLevel(this.Level.Name);
                }
                else
                {
                    // If entering a different room, reset objects in that room
                    Room targetRoom = m_PathToRoom[m_RequestedTarget.RoomPath];
                    if (targetRoom != null)
                    {
                        if (this.Room != targetRoom || !this.MainCharacter.IsAlive)
                        {
                            this.Room = targetRoom;
                            this.Room.Enter(this.MainCharacter);
                            if (!string.IsNullOrEmpty(this.Room.Theme))
                            {
                                this.AudioManager.PlayMusic("LevelPlayScreen" + this.Room.Theme);
                            }
                        }

                        // Enter the door
                        this.MainCharacter.Navigate(this.Room, this.GetDoor(m_RequestedTarget));
                    }
                    else
                    {
                        this.IsQuit = true;
                    }
                }

                // Clear the navigation request
                m_RequestedTarget.Clear();
            }
        }

        /// <summary>
        /// Get the specified door.
        /// </summary>
        /// <returns></returns>
        public Door GetDoor(DoorTarget p_Target)
        {
            return this.Room.GetNamedRoomObject(p_Target.ObjectName) as Door;
        }

        #endregion

        #region Command handlers

        /// <summary>
        /// Register commands.
        /// </summary>
        protected void RegisterCommands()
        {
            CommandManager.Instance.RegisterCommand("levels", this.ConsoleListLevels);
            CommandManager.Instance.RegisterCommand("level", this.ConsoleNavigateToLevel);
            CommandManager.Instance.RegisterCommand("rooms", this.ConsoleListRooms);
            CommandManager.Instance.RegisterCommand("room", this.ConsoleNavigateToRoom);
        }

        /// <summary>
        /// List rooms command handler.
        /// </summary>
        public void ConsoleListRooms(string[] p_Arguments, ref CommandResult p_CommandResult)
        {
            for (int i = 0; i < m_Rooms.Count; i++)
            {
                p_CommandResult.AddOutputLine(string.Format(
                    "{0}: {1}",
                    i + 1,
                    m_Rooms[i].ObjectTypePath.Replace("Room/", string.Empty)
                    ));
            }
        }

        /// <summary>
        /// Navigation to room command handler.
        /// </summary>
        public void ConsoleNavigateToRoom(string[] p_Arguments, ref CommandResult p_CommandResult)
        {
            if (p_Arguments == null || p_Arguments.Length == 0)
            {
                p_CommandResult.AddOutputLine("Usage: 'room <room_number>'");
            }
            else
            {
                int roomNumber = 0;
                IntHelper.TryParse(p_Arguments[0], out roomNumber);
                if (roomNumber == 0 || roomNumber > m_Rooms.Count)
                {
                    p_CommandResult.AddOutputLine(string.Format("'{0}' is not a valid room number", p_Arguments[0]));
                }
                else
                {
                    this.NavigateToRoom(m_Rooms[roomNumber - 1].ObjectTypePath);
                }
            }
        }

        /// <summary>
        /// Navigate to room.
        /// </summary>
        void NavigateToRoom(string p_RoomPath)
        {
            foreach (Room room in m_Rooms)
            {
                if (room.ObjectTypePath == p_RoomPath)
                {
                    Room targetRoom = room;
                    Door targetDoor = targetRoom.Doors[0] as Door;
                    this.RequestNavigation(targetRoom.ObjectTypePath, targetDoor.Name);
                    break;
                }
            }
        }

        /// <summary>
        /// List levels command handler.
        /// </summary>
        public void ConsoleListLevels(string[] p_Arguments, ref CommandResult p_CommandResult)
        {
            PlatformerGame game = PlatformerGame.Instance;
            for (int i = 0; i < game.GameSettings.Levels.Count; i++)
            {
                p_CommandResult.AddOutputLine(string.Format("{0}: {1}", i + 1, game.GameSettings.Levels[i].Name));
            }
        }

        /// <summary>
        /// Navigation to level command handler.
        /// </summary>
        public void ConsoleNavigateToLevel(string[] p_Arguments, ref CommandResult p_CommandResult)
        {
            if (p_Arguments == null || p_Arguments.Length == 0)
            {
                p_CommandResult.AddOutputLine("Usage: 'level <level_number>'");
            }
            else
            {
                int levelNumber = 0;
                IntHelper.TryParse(p_Arguments[0], out levelNumber);

                PlatformerGame game = PlatformerGame.Instance;
                if (levelNumber == 0 || levelNumber > game.GameSettings.Levels.Count)
                {
                    p_CommandResult.AddOutputLine(string.Format("'{0}' is not a valid level number", p_Arguments[0]));
                }
                else
                {
                    game.LoadLevel(game.GameSettings.Levels[levelNumber - 1].Name);
                }
            }
        }

        #endregion
    }
}
