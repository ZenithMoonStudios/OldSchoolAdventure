using Destiny;
using Microsoft.Xna.Framework;
using System;

namespace OSA
{
    public class RunAndJumpMovementProvider : RoomObjectMovementProvider
    {
        #region State enums

        public enum Orientation { Left, Right }
        public enum VerticalStates { Standing, Jumping, Falling }

        public class ActionStates
        {
            public const string StandLeft = "StandLeft";
            public const string StandRight = "StandRight";
            public const string MoveLeft = "MoveLeft";
            public const string MoveRight = "MoveRight";
            public const string JumpLeft = "JumpLeft";
            public const string JumpRight = "JumpRight";
            public const string FallLeft = "FallLeft";
            public const string FallRight = "FallRight";
            public const string WallJumpLeft = "WallJumpLeft";
            public const string WallJumpRight = "WallJumpRight";
            public const string ShootLeft = "ShootLeft";
            public const string ShootRight = "ShootRight";
            public const string Die = "Die";
        }

        #endregion

        /// <summary>
        /// Private state.
        /// </summary>
        Orientation m_Orientation;
        VerticalStates m_VerticalState;
        bool m_IsAttemptingToMove;
        bool m_CanWallJumpNow;
        float m_JumpDistanceRemaining;
        int m_DeathDurationRemaining;
        Vector2 m_ScheduledSpeed;

        Terrain m_CollidedTerrain = null;

        /// <summary>
        /// Private constraints.
        /// </summary>
        RunAndJumpMovementIntelligence m_MovementIntelligence;
        float m_MaxDefaultJumpHeight;
        float m_MaxWallJumpHeight;
        int m_DeathDuration;
        float m_Gravity;
        float m_JumpSpeed;
        float m_MaxVerticalSpeed;
        float m_MaxHorizontalSpeed;
        float m_HorizontalAcceleration;
        float m_FlyAcceleration;
        float m_DeathStartVerticalSpeed;

        /// <summary>
        /// Private constraint accessors.
        /// </summary>
        float Gravity { get { return (m_Gravity + this.GravityShift) * this.Room.ViscosityFactor * this.Room.ViscosityFactor; } }
        float JumpSpeed { get { return m_JumpSpeed * this.Room.ViscosityFactor; } }
        float MaxVerticalSpeed { get { return m_MaxVerticalSpeed * this.Room.ViscosityFactor; } }
        float MaxHorizontalSpeed { get { return m_MaxHorizontalSpeed * this.MaxHorizontalSpeedFactor * this.Room.ViscosityFactor; } }
        float HorizontalAcceleration { get { return m_HorizontalAcceleration * this.Room.ViscosityFactor * this.Room.ViscosityFactor; } }
        float DeathStartVerticalSpeed { get { return m_DeathStartVerticalSpeed * this.Room.ViscosityFactor; } }
        float FlyAcceleration { get { return m_FlyAcceleration * this.Room.ViscosityFactor; } }

        bool HasPower { get { return this.MainCharacter != null && this.MainCharacter.Power != null; } }
        float MaxHorizontalSpeedFactor { get { return this.HasPower ? this.MainCharacter.Power.MaxHorizontalSpeedFactor : 1f; } }
        protected override float GravityShift { get { return (this.RoomObject is MainCharacter && this.HasPower) ? this.MainCharacter.Power.Gravity - m_Gravity : 0f; } }

        /// <summary>
        /// Temporary objects.
        /// </summary>
        static Vector2 s_TempVector;

        /// <summary>
        /// Constructor.
        /// </summary>
        public RunAndJumpMovementProvider(
            RunAndJumpMovementIntelligence p_MovementIntelligence,
            float p_Gravity,
            float p_JumpSpeed,
            float p_MaxDefaultJumpHeight,
            float p_MaxWallJumpHeight,
            float p_MaxVerticalSpeed,
            float p_MaxHorizontalSpeed,
            float p_HorizontalAcceleration,
            float p_FlyAcceleration,
            float p_DeathStartVerticalSpeed,
            int p_DeathDuration,
            GameConditionList p_Conditions
            ) : base(p_Conditions)
        {
            // Set constraints
            m_MovementIntelligence = p_MovementIntelligence;
            m_Gravity = p_Gravity;
            m_JumpSpeed = p_JumpSpeed;
            m_MaxVerticalSpeed = p_MaxVerticalSpeed;
            m_MaxHorizontalSpeed = p_MaxHorizontalSpeed;
            m_HorizontalAcceleration = p_HorizontalAcceleration;
            m_FlyAcceleration = p_FlyAcceleration;
            m_DeathStartVerticalSpeed = p_DeathStartVerticalSpeed;

            m_MaxDefaultJumpHeight = p_MaxDefaultJumpHeight;
            m_MaxWallJumpHeight = p_MaxWallJumpHeight;
            m_DeathDuration = p_DeathDuration;
        }

        /// <summary>
        /// Reset.
        /// </summary>
        public override void DoReset()
        {
            m_VerticalState = VerticalStates.Falling;
            m_Orientation = (m_Velocity.X < 0 ? Orientation.Left : Orientation.Right);
            this.ActionState = (m_Orientation == Orientation.Left) ? ActionStates.StandLeft : ActionStates.StandRight;
            this.IsGroundInFrontOfBaseSolid = true;

            m_DeathDurationRemaining = m_DeathDuration;
            this.UpdateActionStateString();
        }

        protected override void SetCandidateVelocity()
        {
            if (this.RoomObject.IsAlive)
            {
                m_MovementIntelligence.Update(
                    this.RoomObject, m_Orientation, m_VerticalState,
                    this.LeftSide, this.TopSide, this.RightSide, this.BottomSide,
                    this.IsGroundInFrontOfBaseSolid
                    );
                if (m_CanWallJumpNow && m_MovementIntelligence.IsStartJump)
                {
                    this.PerformWallJump();
                }
                else
                {
                    // Update x
                    if (m_ScheduledSpeed.X != 0f)
                    {
                        m_Velocity.X = m_ScheduledSpeed.X;
                        m_ScheduledSpeed.X = 0f;
                    }
                    else
                    {
                        this.UpdateHorizontalVelocity(m_MovementIntelligence.IsMoveLeft, m_MovementIntelligence.IsMoveRight);
                    }
                    // Update y
                    if (m_ScheduledSpeed.Y != 0f)
                    {
                        m_Velocity.Y = m_ScheduledSpeed.Y;
                        m_ScheduledSpeed.Y = 0f;
                        if (m_Velocity.Y < 0)
                        {
                            this.RoomObject.AudioEvent("OnJump");
                        }
                    }
                    else
                    {
                        this.UpdateVerticalVelocity(m_MovementIntelligence.IsStartJump, m_MovementIntelligence.IsStartOrContinueJump);
                        if (m_MovementIntelligence.IsFly)
                        {
                            this.Fly();
                        }
                    }
                }
            }
            else
            {
                // If character just died, set start death speed
                if (m_DeathDurationRemaining == m_DeathDuration)
                {
                    m_Velocity.Y = -this.DeathStartVerticalSpeed;
                }
                this.UpdateVerticalVelocity(false, false);
            }
        }

        protected override void FinalizeCandidateVelocity()
        {
            this.ConstrainMaxVelocity();

            // If standing and velocity is less than 0, consider to be falling
            if (this.RoomObject.IsAlive)
            {
                if (m_Velocity.Y > 0 || (m_VerticalState == VerticalStates.Standing && m_Velocity.Y < 0 && this.ActiveTileAcceleration.Y < 0))
                {
                    m_VerticalState = VerticalStates.Falling;
                }
            }
        }

        void ConstrainMaxVelocity()
        {
            // Do not let velocity exceed allowed maximums
            m_Velocity.X = MathHelper.Clamp(m_Velocity.X, -this.MaxHorizontalSpeed, this.MaxHorizontalSpeed);
            m_Velocity.Y = MathHelper.Clamp(m_Velocity.Y, -this.MaxVerticalSpeed, this.MaxVerticalSpeed);
        }

        protected override void UpdateState(ref bool p_DoInteract)
        {
            // This method is called once per frame, so update frames since last death here
            MainCharacter mainCharacter = this.RoomObject as MainCharacter;
            if (mainCharacter != null)
            {
                mainCharacter.FramesSinceLastDeath++;
            }

            if (this.RoomObject.IsAlive)
            {
                // Object is alive, so evaluate the surroundings for what can be done next move
                this.UpdateOrientation(m_MovementIntelligence.IsMoveLeft, m_MovementIntelligence.IsMoveRight);
                this.EvaluateSurroundings();
                this.EvaluateCanWallJump();

                m_IsAttemptingToMove = m_MovementIntelligence.IsMoveLeft || m_MovementIntelligence.IsMoveRight;
                p_DoInteract = m_MovementIntelligence.IsStartAction;
            }
            else
            {
                // Continue to die, until remaining death time runs out
                if (--m_DeathDurationRemaining == 0)
                {
                    this.RoomObject.FinishDeath();
                    if (mainCharacter != null)
                    {
                        mainCharacter.FramesSinceLastDeath = 0;
                        mainCharacter.FramesUntilNextDeath = 0;
                    }
                }
                else
                {
                    if (mainCharacter != null)
                    {
                        mainCharacter.FramesUntilNextDeath = m_DeathDurationRemaining;
                    }
                }
                p_DoInteract = false;
            }

            this.UpdateActionStateString();
        }

        void EvaluateCanWallJump()
        {
            m_CanWallJumpNow =
                (m_VerticalState == VerticalStates.Jumping || m_VerticalState == VerticalStates.Falling) &&
                (this.LeftSide.IsSolid || this.RightSide.IsSolid);
        }

        /// <summary>
        /// Perform a wall jump.
        /// </summary>
        void PerformWallJump()
        {
            if (this.LeftSide.IsSolid)
            {
                // Jump left
                m_Orientation = Orientation.Right;
                m_Velocity.X = this.MaxHorizontalSpeed;
            }
            else if (this.RightSide.IsSolid)
            {
                // Jump right
                m_Orientation = Orientation.Left;
                m_Velocity.X = -this.MaxHorizontalSpeed;
            }
            m_Velocity.Y = -this.JumpSpeed;
            m_VerticalState = VerticalStates.Jumping;

            // Set jump height
            m_JumpDistanceRemaining = m_MaxWallJumpHeight;

            // Play audio
            this.RoomObject.AudioEvent("OnWallJump");
        }

        /// <summary>
        /// Fly.
        /// </summary>
        void Fly()
        {
            m_Velocity.Y -= this.FlyAcceleration;
            m_VerticalState = VerticalStates.Jumping;
        }

        void ScheduleSpeed(Vector2 p_Speed)
        {
            m_ScheduledSpeed = p_Speed;
        }

        #region Orientation

        private void UpdateOrientation(bool isMoveLeft, bool isMoveRight)
        {
            if (isMoveLeft && !isMoveRight)
            {
                m_Orientation = Orientation.Left;
            }
            else if (isMoveRight && !isMoveLeft)
            {
                m_Orientation = Orientation.Right;
            }
        }

        #endregion

        void UpdateHorizontalVelocity(bool p_IsMoveLeft, bool p_IsMoveRight)
        {
            // Calculate friction
            float totalSideVelocity = this.TopSide.Velocity + this.BottomSide.Velocity;
            float totalFriction = this.TopSide.Friction + this.BottomSide.Friction;
            float relativeVelocity = (m_Velocity.X - totalSideVelocity);
            float friction = Math.Min(Math.Abs(totalFriction), Math.Abs(relativeVelocity)) * this.Room.ViscosityFactor * this.Room.ViscosityFactor;

            // Calculate acceleration
            float acceleration = 0f;
            bool isStanding = m_VerticalState == VerticalStates.Standing;
            if (p_IsMoveLeft)
            {
#if (MOBILE)
				//SJ - consider linking modifier to difficulty
				acceleration = (this.Room.InputState.AccelerometerReading.X + 0.0f) - friction;
#else
                acceleration = -this.HorizontalAcceleration - friction;
#endif
            }
            else if (p_IsMoveRight)
            {
#if (MOBILE)
				acceleration = (this.Room.InputState.AccelerometerReading.X - 0.0f) + friction;
#else
                acceleration = this.HorizontalAcceleration + friction;
#endif
            }
            else
            {
                if (relativeVelocity > 0f)
                {
                    acceleration = -friction;
                }
                else if (relativeVelocity < 0f)
                {
                    acceleration = friction;
                }
            }

            // Update velocity
            m_Velocity.X += acceleration;
        }

        void UpdateVerticalVelocity(bool p_IsStartJump, bool p_IsStartOrContinueJump)
        {
            // Handle jumping
            if (m_VerticalState == VerticalStates.Jumping)
            {
                if (!p_IsStartOrContinueJump || m_JumpDistanceRemaining <= 0f)
                {
                    m_VerticalState = VerticalStates.Falling;
                }
            }
            if (m_VerticalState == VerticalStates.Standing)
            {
                if (p_IsStartJump)
                {
                    m_JumpDistanceRemaining = m_MaxDefaultJumpHeight;
                    m_VerticalState = VerticalStates.Jumping;
                    this.RoomObject.AudioEvent("OnJump");
                }
            }

            // Calculate friction
            float friction = 0f;
            float totalSideVelocity = this.LeftSide.Velocity + this.RightSide.Velocity;
            float totalFriction = this.LeftSide.Friction + this.RightSide.Friction;
            float relativeVelocity = (m_Velocity.Y - totalSideVelocity);
            if (relativeVelocity > 0f)
            {
                friction = Math.Max(-totalFriction, -relativeVelocity) * this.Room.ViscosityFactor * this.Room.ViscosityFactor;
            }
            else if (relativeVelocity < 0f)
            {
                friction = Math.Min(totalFriction, -relativeVelocity) * this.Room.ViscosityFactor * this.Room.ViscosityFactor;
            }

            // Calculate acceleration
            float acceleration = 0f;
            if (m_VerticalState == VerticalStates.Jumping)
            {
                acceleration = -this.JumpSpeed;
            }
            else
            {
                acceleration = this.Gravity;
            }

            // Update velocity
            m_Velocity.Y += friction + acceleration;

            // If character is picked up from the ground without jumping, even though
            // velocity is upwards, set status to falling
            if (m_VerticalState == VerticalStates.Standing && friction < 0f && m_Velocity.Y < 0f)
            {
                m_VerticalState = VerticalStates.Falling;
            }
        }

        protected void UpdateActionStateString()
        {
            if (this.RoomObject.IsAlive)
            {
                bool isFacingLeft = (m_Orientation == Orientation.Left);
                if (this.RoomObject.IsShooting)
                {
                    this.ActionState = isFacingLeft ? ActionStates.ShootLeft : ActionStates.ShootRight;
                }
                else if (m_CanWallJumpNow)
                {
                    this.ActionState = isFacingLeft ? ActionStates.WallJumpLeft : ActionStates.WallJumpRight;
                }
                else if (m_VerticalState == VerticalStates.Jumping)
                {
                    this.ActionState = isFacingLeft ? ActionStates.JumpLeft : ActionStates.JumpRight;
                }
                else if (m_VerticalState == VerticalStates.Standing)
                {
                    if (m_IsAttemptingToMove)
                    {
                        this.ActionState = isFacingLeft ? ActionStates.MoveLeft : ActionStates.MoveRight;
                    }
                    else
                    {
                        this.ActionState = isFacingLeft ? ActionStates.StandLeft : ActionStates.StandRight;
                    }
                }
                else if (m_VerticalState == VerticalStates.Falling)
                {
                    this.ActionState = isFacingLeft ? ActionStates.FallLeft : ActionStates.FallRight;
                }
            }
            else
            {
                this.ActionState = ActionStates.Die;
            }
        }

        protected override void PreCommitMoveY(float p_Offset)
        {
            if (p_Offset < 0f)
            {
                m_JumpDistanceRemaining += p_Offset;
            }
        }

        #region Collision handlers

        /// <summary>
        /// Handle obstacle collision.
        /// </summary>
        protected override void HandleObstacleCollision(Obstacle p_Obstacle, SurfaceDirections p_SurfaceDirection)
        {
            SurfaceDirections obstacleSurfaceDirection = SurfaceDirectionsHelper.Opposite(p_SurfaceDirection);
            SurfaceInformation collisionSide = p_Obstacle.GetSide(obstacleSurfaceDirection);

            // If object is weak on collision side, kill it
            if (collisionSide.IsDie)
            {
                p_Obstacle.StartDeath();
            }

            //  If collision side has a tangent speed, apply it
            if (collisionSide.TangentSpeed != 0f)
            {
                // X
                if (obstacleSurfaceDirection == SurfaceDirections.Left) { s_TempVector.X = -collisionSide.TangentSpeed; }
                else if (obstacleSurfaceDirection == SurfaceDirections.Right) { s_TempVector.X = collisionSide.TangentSpeed; }
                else { s_TempVector.X = 0f; }
                // Y
                if (obstacleSurfaceDirection == SurfaceDirections.Top) { s_TempVector.Y = -collisionSide.TangentSpeed; }
                else if (obstacleSurfaceDirection == SurfaceDirections.Bottom) { s_TempVector.Y = collisionSide.TangentSpeed; }
                else { s_TempVector.Y = 0f; }

                this.ScheduleSpeed(s_TempVector);
            }

            // If room object hits its head, regardless of whether there is a tangent speed it is now falling
            if (p_SurfaceDirection == SurfaceDirections.Top)
            {
                m_VerticalState = VerticalStates.Falling;
            }
            else if (p_SurfaceDirection == SurfaceDirections.Bottom && collisionSide.TangentSpeed == 0f)
            {
                m_VerticalState = VerticalStates.Standing;
            }
        }

        protected override void HandleTileCollision(SurfaceDirections p_CollisionSurfaceDirection)
        {
            if (p_CollisionSurfaceDirection == SurfaceDirections.Top) m_VerticalState = VerticalStates.Falling;
            else if (p_CollisionSurfaceDirection == SurfaceDirections.Bottom) m_VerticalState = VerticalStates.Standing;
        }

        protected override void HandleTerrainCollision(Terrain p_Terrain)
        {
            TerrainPosition terrainPosition = p_Terrain.TerrainPosition;
            if (terrainPosition == TerrainPosition.Top) m_VerticalState = VerticalStates.Falling;
            else if (terrainPosition == TerrainPosition.Bottom) m_VerticalState = VerticalStates.Standing;

            m_CollidedTerrain = p_Terrain;
        }

        #endregion

        protected override float GetGroundInFrontOfBaseX()
        {
            // Determine position for evaluating the ground in front of character's feet
            return (m_Orientation == Orientation.Left) ? this.Left - 1f : this.Right + 1f;
        }

        /// <summary>
        /// Audio manager.
        /// </summary>
        private AudioManager AudioManager { get { return PlatformerGame.Instance.AudioManager; } }
    }

    #region Xml Reader

    /// <summary>
    /// Xml room object movement provider p_Reader.
    /// </summary>
    public class XmlRunAndJumpMovementProviderReader : XmlRoomObjectMovementProviderReader
    {
        public XmlRunAndJumpMovementProviderReader()
        {
        }

        public override RoomObjectMovementProvider CreateInstance(OSATypes.TemplateMovementProvider p_Provider, OSATypes.TemplateMovementProvider p_RoomTemplateObject)
        {
            OSATypes.TemplateMovementProvider provider = p_RoomTemplateObject != null ? p_RoomTemplateObject : null;
            // Initialize movement intelligence
            RunAndJumpMovementIntelligence movementIntelligence = null;
            string intelligenceAlgorithm = provider.IntelligenceAlgorithm != null ? provider.IntelligenceAlgorithm : "UserInput";

            switch (intelligenceAlgorithm)
            {
                case "TurnOnCollision":
                    movementIntelligence = new RunAndJumpTurnOnCollisionIntelligence();
                    break;
                case "TurnOnHoleOrCollision":
                    movementIntelligence = new RunAndJumpTurnOnHoleOrCollisionIntelligence();
                    break;
                case "TurnOnCollisionAndUserJumpInput":
                    movementIntelligence = new RunAndJumpTurnOnCollisionAndUserJumpInputIntelligence();
                    break;
                case "FollowPlayer":
                    movementIntelligence = new RunAndJumpFollowPlayerIntelligence();
                    break;
                case "JumpOverHole":
                    movementIntelligence = new RunAndJumpJumpOverHoleIntelligence();
                    break;
                default:
                    movementIntelligence = new RunAndJumpUserInputIntelligence();
                    break;
            }
            float gravity = float.TryParse(provider.Gravity, out gravity) ? float.Parse(provider.Gravity) : 0.5f;
            float jumpSpeed = float.TryParse(provider.JumpSpeed, out jumpSpeed) ? float.Parse(provider.JumpSpeed) : 7f;
            float maxDefaultJumpHeight = float.TryParse(provider.MaxDefaultJumpHeight, out maxDefaultJumpHeight) ? float.Parse(provider.MaxDefaultJumpHeight) : 90f;
            float maxWallJumpHeight = float.TryParse(provider.MaxWallJumpHeight, out maxWallJumpHeight) ? float.Parse(provider.MaxWallJumpHeight) : 40f;
            float maxVerticalSpeed = float.TryParse(provider.MaxVerticalSpeed, out maxVerticalSpeed) ? float.Parse(provider.MaxVerticalSpeed) : 7f;
            float maxHorizontalSpeed = float.TryParse(provider.MaxHorizontalSpeed, out maxHorizontalSpeed) ? float.Parse(provider.MaxHorizontalSpeed) : 5f;
            float horizontalAcceleration = float.TryParse(provider.HorizontalAcceleration, out horizontalAcceleration) ? float.Parse(provider.HorizontalAcceleration) : 0.2f;
            float flyAcceleration = float.TryParse(provider.FlyAcceleration, out flyAcceleration) ? float.Parse(provider.FlyAcceleration) : 0.1f;
            float deathStartVerticalSpeed = float.TryParse(provider.DeathStartVerticalSpeed, out deathStartVerticalSpeed) ? float.Parse(provider.DeathStartVerticalSpeed) : 7f;
            int deathDuration = int.TryParse(provider.DeathDuration, out deathDuration) ? int.Parse(provider.DeathDuration) : 600;

            GameConditionList conditions = null;
            if (p_Provider != null)
            {
                conditions = XmlGameConditionListReader.Load(p_Provider.Conditions);
            }

            return new RunAndJumpMovementProvider(
                movementIntelligence,
                gravity, jumpSpeed, maxDefaultJumpHeight, maxWallJumpHeight,
                maxVerticalSpeed, maxHorizontalSpeed, horizontalAcceleration, flyAcceleration,
                deathStartVerticalSpeed, deathDuration,
                conditions
                );
        }
    }

    #endregion
}
