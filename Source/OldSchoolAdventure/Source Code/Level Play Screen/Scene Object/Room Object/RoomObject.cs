using Destiny;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace OSA
{
    /// <summary>
    /// Room object base class.
    /// </summary>
    public class RoomObject : SceneObject
    {
        /// <summary>
        /// Context.
        /// </summary>
        public Room Room { get; protected set; }
        public RoomObject Parent { get; protected set; }

        /// <summary>
        /// Identifiers.
        /// </summary>
        public string Name { get; protected set; }
        public string RepeatableId { get; private set; }
        public OSATypes.TemplateMovementProvider ObjectMovementProvider { get; private set; }

        /// <summary>
        /// Properties.
        /// </summary>
        public VerticalAlignment VerticalAlignment { get; private set; }
        public ShootInformation ShootInformation { get; private set; }
        public GameConditionList Conditions { get; private set; }
        public int Offense { get; protected set; }
        public int Defense { get; protected set; }
        protected StringToStringDictionary AudioCues { get; private set; }

        public bool CanFly { get; protected set; }
        public bool CanAccelerateFromTile { get; protected set; }

        /// <summary>
        /// Rendering.
        /// </summary>
        public string RenderMode { get; private set; }

        /// <summary>
        /// State.
        /// </summary>
        public enum LifeStates { Alive, Dying, Dead }

        protected Vector2 m_StartPosition;
        protected Vector2 m_StartVelocity;
        public Vector2 StartGridPosition { get; private set; }

        public LifeStates LifeState { get; protected set; }
        protected bool m_DoInteract = false;

        public bool IsAlive { get { return this.LifeState == LifeStates.Alive; } }
        public bool DoInteract { get { return m_DoInteract; } }
        public bool IsShooting { get; private set; }

        /// <summary>
        /// Shoot state.
        /// </summary>
        int m_ShootStartFrame;
        int m_ShootFinishFrame;

        /// <summary>
        /// Movement provider.
        /// </summary>
        RoomObjectMovementProvider m_MovementProvider;

        /// <summary>
        /// Constructor.
        /// </summary>
        public RoomObject(RoomObjectInitialization p_Init) : this(
            p_Init.ObjectTypePath,
            p_Init.Size,
            p_Init.Position,
            p_Init.GridPosition,
            p_Init.Velocity,
            p_Init.Name,
            p_Init.MovementProvider,
            p_Init.VerticalAlignment,
            p_Init.RenderMode,
            p_Init.ObjectMovementProvider,
            p_Init.ShootInformation,
            p_Init.Offense,
            p_Init.Defense,
            p_Init.AudioCues,
            p_Init.Conditions
            )
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public RoomObject(
            string p_ObjectTypePath,
            Vector2 p_Size,
            Vector2 p_StartPosition,
            Vector2 p_StartGridPosition,
            Vector2 p_StartVelocity,
            string p_Name,
            RoomObjectMovementProvider p_MovementProvider,
            VerticalAlignment p_VerticalAlignment,
            string p_RenderMode,
            OSATypes.TemplateMovementProvider p_ObjectMovementProvider,
            ShootInformation p_ShootInformation,
            int p_Offense,
            int p_Defense,
            StringToStringDictionary p_AudioCues,
            GameConditionList p_Conditions
            ) : base(p_ObjectTypePath, p_Size, p_StartPosition)
        {
            // Remember the start position
            this.SetStartPosition(p_StartGridPosition, p_StartPosition);
            m_StartVelocity.X = p_StartVelocity.X;
            m_StartVelocity.Y = p_StartVelocity.Y;

            this.Name = p_Name;
            if (string.IsNullOrEmpty(this.Name))
            {
                this.Name = Guid.NewGuid().ToString();
            }

            this.ObjectMovementProvider = p_ObjectMovementProvider;

            m_MovementProvider = p_MovementProvider;
            if (m_MovementProvider != null)
            {
                m_MovementProvider.Initialize(this);
            }
            this.VerticalAlignment = p_VerticalAlignment;
            this.RenderMode = p_RenderMode;
            this.ShootInformation = p_ShootInformation;
            this.Offense = p_Offense;
            this.Defense = p_Defense;

            this.CanFly = false;
            this.CanAccelerateFromTile = true;

            // Copy audio cues
            this.AudioCues = new StringToStringDictionary();
            if (p_AudioCues != null)
            {
                foreach (string key in p_AudioCues.Keys)
                {
                    this.AudioCues.Add(key, p_AudioCues[key]);
                }
            }

            // Copy conditions
            this.Conditions = new GameConditionList();
            if (p_Conditions != null)
            {
                for (int i = 0; i < p_Conditions.Count; i++)
                {
                    this.Conditions.Add(p_Conditions[i]);
                }
            }

            this.UpdateActionState();
        }

        public void SetStartPosition(Vector2 p_GridPosition, Vector2 p_Position)
        {
            this.StartGridPosition = p_GridPosition;
            m_StartPosition.X = p_Position.X;
            m_StartPosition.Y = p_Position.Y;
        }

        public void BindProjectiles(RoomObjectList p_Projectiles)
        {
            for (int i = 0; i < p_Projectiles.Count; i++)
            {
                p_Projectiles[i].Parent = this;
                this.SceneObjects.Add(p_Projectiles[i]);
            }
        }

        public void BindToRoom(Room p_Room)
        {
            this.Room = p_Room;
            this.RepeatableId = this.Room.GetRepeatableId();
            for (int i = 0; i < this.SceneObjects.Count; i++)
            {
                RoomObject projectile = this.SceneObjects[i] as RoomObject;
                if (projectile != null)
                {
                    projectile.BindToRoom(p_Room);
                }
            }
        }

        /// <summary>
        /// Reset object to initial state.
        /// </summary>
        public override void Reset()
        {
            this.Reset(m_StartPosition, m_StartVelocity);
        }

        /// <summary>
        /// Reset object to specified state.
        /// </summary>
        public void Reset(Vector2 p_StartPosition, Vector2 p_StartVelocity)
        {
            m_LastDirectorInstruction = null;

            CollectibleStore store = (this.Room != null ? this.Room.MainCharacter.Store : null);
            bool isCollected = (store != null && this is Collectible && store.HasCollected(this as Collectible));
            bool isMeetConditions = (store == null || this.Conditions.IsTrue(store));
            if (isCollected || !isMeetConditions)
            {
                this.LifeState = LifeStates.Dead;
            }
            else
            {
                // When resetting a door, remember if it has ever been unlocked
                Door door = this as Door;
                if (door != null && store != null && store.HasUnlocked(door))
                {
                    door.ForceUnlock();
                }

                // Normal reset logic
                this.LifeState = LifeStates.Alive;
                for (int i = 0; i < this.SceneObjects.Count; i++)
                {
                    RoomObject projectile = this.SceneObjects[i] as RoomObject;
                    if (projectile != null)
                    {
                        projectile.LifeState = LifeStates.Dead;
                    }
                }
                m_Position.X = m_PreviousPosition.X = p_StartPosition.X;
                m_Position.Y = m_PreviousPosition.Y = p_StartPosition.Y;
                m_Velocity.X = p_StartVelocity.X;
                m_Velocity.Y = p_StartVelocity.Y;
                if (m_MovementProvider != null)
                {
                    m_MovementProvider.Reset(m_Position, m_Velocity);
                }

                // Initialize shooting
                if (this.ShootInformation != null)
                {
                    this.IsShooting = false;
                    this.CalculateNextShootFrame();
                }
            }
        }

        /// <summary>
        /// Update.
        /// </summary>
        protected override void Update()
        {
            if (this.LifeState != LifeStates.Dead)
            {
                // Process shooting
                this.UpdateShoot();

                // Process movement
                if (m_MovementProvider != null)
                {
                    m_MovementProvider.Move(ref m_Position, m_Velocity, ref m_DoInteract);
                }
            }
        }

        static Vector2 s_TempPosition;
        static Vector2 s_TempVelocity;
        void UpdateShoot()
        {
            if (this.LifeState == LifeStates.Alive && this.ShootInformation != null)
            {
                if (this.ActiveFrames == m_ShootStartFrame)
                {
                    for (int i = 0; i < this.SceneObjects.Count; i++)
                    {
                        RoomObject roomObject = this.SceneObjects[i] as RoomObject;
                        if (roomObject != null && roomObject.LifeState == LifeStates.Dead)
                        {
                            // Process X axis
                            if (this.ShootInformation.FlipOnNegativeX && m_Velocity.X < 0)
                            {
                                s_TempPosition.X = this.Right - this.ShootInformation.StartPosition.X - (roomObject.Width / 2);
                                s_TempVelocity.X = -this.ShootInformation.StartVelocity.X;
                            }
                            else
                            {
                                s_TempPosition.X = this.Left + this.ShootInformation.StartPosition.X - (roomObject.Width / 2);
                                s_TempVelocity.X = this.ShootInformation.StartVelocity.X;
                            }
                            // Process Y axis
                            if (this.ShootInformation.FlipOnNegativeY && m_Velocity.Y < 0)
                            {
                                s_TempPosition.Y = this.Bottom - this.ShootInformation.StartPosition.Y - (roomObject.Height / 2);
                                s_TempVelocity.Y = -this.ShootInformation.StartVelocity.Y;
                            }
                            else
                            {
                                s_TempPosition.Y = this.Top + this.ShootInformation.StartPosition.Y - (roomObject.Height / 2);
                                s_TempVelocity.Y = this.ShootInformation.StartVelocity.Y;
                            }

                            // Calculate start position
                            roomObject.Reset(s_TempPosition, s_TempVelocity);
                            this.IsShooting = true;
                            break;
                        }
                    }
                }
                else if (this.ActiveFrames == m_ShootFinishFrame)
                {
                    this.IsShooting = false;
                    this.CalculateNextShootFrame();
                }
            }
        }

        /// <summary>
        /// Draw.
        /// </summary>
        protected override void DoDraw(DrawManager p_DrawManager)
        {
            if (this.LifeState != LifeStates.Dead)
            {
                if (string.IsNullOrEmpty(this.ActionState))
                {
                    this.UpdateActionState();
                }
                p_DrawManager.Draw(this);
            }
        }

        protected override void UpdateActionState()
        {
            if (this.RenderMode == "Movement" && m_MovementProvider != null)
            {
                this.ActionState = m_MovementProvider.ActionState;
            }
            else if (this.RenderMode == "Collision")
            {
                this.ActionState = this.CollisionActionState;
            }
        }

        /// <summary>
        /// Collision action state.
        /// </summary>
        protected string CollisionActionState { get; set; }

        /// <summary>
        /// Start death.
        /// </summary>
        public virtual void StartDeath()
        {
            if (this.IsAlive)
            {
                this.LifeState = LifeStates.Dying;
                this.AudioEvent("OnDie");
            }
        }

        /// <summary>
        /// Finish death.
        /// </summary>
        public virtual void FinishDeath()
        {
            this.LifeState = LifeStates.Dead;
        }

        /// <summary>
        /// Play audio for the specified event.
        /// </summary>
        public void AudioEvent(string p_EventName)
        {
            if (this.AudioCues.ContainsKey(p_EventName))
            {
                this.AudioManager.PlayCue(this.AudioCues[p_EventName]);
            }
        }

        /// <summary>
        /// Calculate the frame number of the next shot.
        /// </summary>
        void CalculateNextShootFrame()
        {
            int waitRange = this.ShootInformation.MaxWait - this.ShootInformation.MinWait;
            int waitTime = this.ShootInformation.MinWait;
            if (waitRange != 0)
            {
                waitTime += MathHelper2.Random(waitRange + 1);
            }
            m_ShootStartFrame = this.ActiveFrames + waitTime;
            m_ShootFinishFrame = m_ShootStartFrame + this.ShootInformation.Duration;
        }

        protected int ActiveFrames { get { return this.Room.Screen.ActiveFrames; } }
        protected AudioManager AudioManager { get { return PlatformerGame.Instance.AudioManager; } }
    }

    /// <summary>
    /// Room object list class.
    /// </summary>
    public class RoomObjectList : List<RoomObject> { }

    /// <summary>
    /// String to room object dictionary class.
    /// </summary>
    public class StringToRoomObjectDictionary : Dictionary<string, RoomObject> { }
}
