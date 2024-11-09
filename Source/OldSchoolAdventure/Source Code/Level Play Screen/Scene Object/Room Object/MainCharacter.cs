using Destiny;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace OSA
{
    /// <summary>
    /// Main character class.
    /// </summary>
    public class MainCharacter : RoomObject
    {
        public CollectibleStore Store { get; private set; }
        public CollectiblePower Power { get { return this.Store.Power; } }
        public DoorTarget LastDoor { get; private set; }
        LevelPlayScreen m_PlayScreen;

        List<string> m_ItemsToShow;
        Dictionary<string, Vector2> m_PathToSize;
        Dictionary<string, VerticalAlignment> m_PathToVerticalAlignment;

        const int c_PositionHistoryArraySize = 16;
        Vector2[] m_PositionHistory = new Vector2[c_PositionHistoryArraySize];
        int m_PositionHistoryStartIndex;
        int m_PositionHistoryFinishIndex;

        // Death status, for fading in and out play screen
        public int FramesSinceLastDeath { get; set; }
        public int FramesUntilNextDeath { get; set; }

        int m_DefaultOffense;
        int m_DefaultDefense;
        const int c_HelmetDefense = 1;

        string m_OverridePath;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainCharacter(CollectibleStore p_Store, RoomObjectInitialization p_Init) : base(p_Init)
        {
            this.Store = p_Store;
            this.LastDoor = new DoorTarget();

            m_DefaultOffense = this.Offense;
            m_DefaultDefense = this.Defense;
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public void Initialize(LevelPlayScreen p_PlayScreen)
        {
            m_Velocity.X = 0f;
            m_Velocity.Y = 0f;
            m_PlayScreen = p_PlayScreen;

            // Initialize collectible items
            XmlCollectibleReader reader = new XmlCollectibleReader();
            m_ItemsToShow = new StringList();
            m_PathToSize = new Dictionary<string, Vector2>();
            m_PathToVerticalAlignment = new Dictionary<string, VerticalAlignment>();
            List<String> Collectables = new List<string>();
            Collectables = p_PlayScreen.DrawManager.ContentManager.Load<Destiny.Collectables>(@"Collectible\CollectablesList");
            foreach (string filename in Collectables)
            {
                string path = "Collectible/" + filename;

                // Load template properties
                Vector2 size;
                Vector2 startVelocity;
                VerticalAlignment verticalAlignment;
                RoomObjectTemplate template = this.RoomObjectTemplateManager.Get(path);
                XmlRoomObjectReader.GetTemplateProperties(template.RoomTemplateObject, out size, out startVelocity, out verticalAlignment);

                // Register if show on carry
                Collectible collectible = reader.CreateInstance(path, null, template.RoomTemplateObject, Vector2.Zero, null) as Collectible;
                if (collectible.ShowCarry)
                {
                    m_ItemsToShow.Add(path);
                    m_PathToSize.Add(path, size);
                    m_PathToVerticalAlignment.Add(path, verticalAlignment);
                    p_PlayScreen.DrawManager.RegisterType(path);
                }

                // Register hero skins
                if (collectible.Power != null)
                {
                    p_PlayScreen.DrawManager.RegisterType(collectible.Power.OwnerPath);
                }
            }

            // Initialize death transition states
            this.FramesSinceLastDeath = 0;
            this.FramesUntilNextDeath = 0;
        }

        #region Navigation

        /// <summary>
        /// Set the spawn point.
        /// </summary>
        public void SetSpawnPoint(DoorTarget p_Target)
        {
            this.LastDoor.Set(m_PlayScreen.GetDoor(p_Target));
        }

        /// <summary>
        /// Submit a request to navigate to the specified target location.
        /// </summary>
        /// <param name="p_Target">Target.</param>
        public void RequestNavigation(DoorTarget p_Target)
        {
            m_PlayScreen.RequestNavigation(p_Target);
        }

        public void Navigate(Room p_Room, Door p_Door)
        {
            // Set position based on the alignment of the target object
            // If a door is bottom aligned, to sit neatly on a platform, the character
            // should also set neatly on the platform, and if another warp is center
            // aligned, the character should appeat in the middle of that object
            int verticalPosition = 0;
            if (p_Door.VerticalAlignment == VerticalAlignment.Top)
            {
                verticalPosition = (int)p_Door.Top;
            }
            else if (p_Door.VerticalAlignment == VerticalAlignment.Bottom)
            {
                verticalPosition = (int)(p_Door.Top + p_Door.Height - this.Height);
            }
            else
            {
                verticalPosition = (int)(p_Door.Top + ((p_Door.Height - this.Height) / 2));
            }
            m_Position = new Vector2(
                p_Door.Left + ((p_Door.Width - this.Width) / 2),
                verticalPosition
                );

            bool isSameRoom = this.Room != null && this.Room.ObjectTypePath == p_Room.ObjectTypePath;
            if (!isSameRoom)
            {
                this.Room = p_Room;
            }

            // Some doors should never act as spawn points
            if (p_Door.IsValidRespawnLocation)
            {
                this.LastDoor.Set(p_Door);
            }

            // If dead, this will bring the player back to life
            this.LifeState = LifeStates.Alive;

            // Clear position history
            this.ClearPositionHistory();
        }

        void ClearPositionHistory()
        {
            m_PositionHistoryStartIndex = 0;
            m_PositionHistoryFinishIndex = 0;
            m_PositionHistory[m_PositionHistoryStartIndex].X = m_Position.X;
            m_PositionHistory[m_PositionHistoryStartIndex].Y = m_Position.Y;
        }

        #endregion

        #region Conversation

        /// <summary>
        /// Speak.
        /// </summary>
        public void Speak(SpeakerConversation p_Converation)
        {
            if (this.IsAlive)
            {
                m_PlayScreen.ScreenManager.AddScreen(new PlayConversationScreen(m_PlayScreen, p_Converation));
            }
        }

        #endregion

        #region Death

        /// <summary>
        /// Finish death.
        /// </summary>
        public override void FinishDeath()
        {
            m_PlayScreen.RequestNavigation(this.LastDoor);
        }

        #endregion

        public void WarpToPosition(Vector2 p_Position)
        {
            m_Position.X = p_Position.X;
            m_Position.Y = p_Position.Y;
        }

        protected override void Update()
        {
            if (this.Power != null)
            {
                this.CanFly = this.Power.CanFly;
                this.CanAccelerateFromTile = this.Power.CanAccelerateFromTile;
                this.Offense = this.Power.Offense;
                this.Defense = this.Power.Defense;
            }
            else
            {
                this.CanFly = false;
                this.CanAccelerateFromTile = true;
                this.Offense = 0;
                this.Defense = 0;
            }

            // Update as normal
            base.Update();

            // Update position history
            m_PositionHistoryStartIndex++;
            m_PositionHistoryStartIndex %= c_PositionHistoryArraySize;
            if (m_PositionHistoryStartIndex == m_PositionHistoryFinishIndex)
            {
                m_PositionHistoryFinishIndex++;
                m_PositionHistoryFinishIndex %= c_PositionHistoryArraySize;
            }
            m_PositionHistory[m_PositionHistoryStartIndex].X = m_Position.X;
            m_PositionHistory[m_PositionHistoryStartIndex].Y = m_Position.Y;

            // Populate override path
            m_OverridePath = (this.Power != null) ? this.Power.OwnerPath : string.Empty;
        }

        protected override void DoDraw(DrawManager p_DrawManager)
        {
            if (this.LifeState != LifeStates.Dead && !string.IsNullOrEmpty(this.ActionState))
            {
                // Draw collectibles
                int itemIndex = 1;
                for (int i = 0; i < m_ItemsToShow.Count; i++)
                {
                    string itemPath = m_ItemsToShow[i];
                    if (this.Store.Has(itemPath))
                    {
                        // Work out position based on history
                        int historyLength = 5 * itemIndex;
                        int positionHistoryIndex = 0;
                        if (m_PositionHistoryStartIndex == m_PositionHistoryFinishIndex)
                        {
                            // Hasn't moved yet, so use first position
                        }
                        else if (m_PositionHistoryStartIndex > m_PositionHistoryFinishIndex)
                        {
                            // Start > finish only when finish = 0
                            positionHistoryIndex = Math.Max(0, m_PositionHistoryStartIndex - historyLength);
                        }
                        else
                        {
                            // Start < finish when history is full and data is wrapping
                            positionHistoryIndex = m_PositionHistoryStartIndex - historyLength;
                            if (positionHistoryIndex < 0)
                            {
                                positionHistoryIndex += c_PositionHistoryArraySize;
                            }
                        }

                        // Draw collectible
                        Vector2 size;
                        Vector2 startVelocity;
                        VerticalAlignment verticalAlignment;
                        RoomObjectTemplate template = this.RoomObjectTemplateManager.Get(itemPath);
                        XmlRoomObjectReader.GetTemplateProperties(template.RoomTemplateObject, out size, out startVelocity, out verticalAlignment);

                        RoomObjectTemplate itemTemplate = this.RoomObjectTemplateManager.Get(itemPath);
                        p_DrawManager.DrawRoomObject(
                            itemPath,
                            (int)(m_PositionHistory[positionHistoryIndex].X + ((this.Width - m_PathToSize[itemPath].X) / 2)),
                            (int)(m_PositionHistory[positionHistoryIndex].Y + ((this.Height - m_PathToSize[itemPath].Y) / 2)),
                            true,               // Is world position?
                            "Default",          // Action state
                            this.ActiveFrames,  // Action elapsed time
                            this.Camera,
                            this.Room,
                            p_DrawManager.SpriteBatch
                            );

                        itemIndex++;
                    }
                }

                // Draw hero
                p_DrawManager.DrawRoomObject(this, m_OverridePath);
            }
        }

        RoomObjectTemplateManager RoomObjectTemplateManager { get { return m_PlayScreen.RoomObjectTemplateManager; } }
        Camera Camera { get { return m_PlayScreen.Camera; } }
    }
}
