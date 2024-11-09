namespace OSA
{
    /// <summary>
    /// Linear projectile movement provider.
    /// </summary>
    public class LinearProjectileMovementProvider : RoomObjectMovementProvider
    {
        public class ActionStates
        {
            public const string MoveLeft = "MoveLeft";
            public const string MoveRight = "MoveRight";
            public const string Die = "Die";
        }

        int m_DeathDuration;
        int m_DeathDurationRemaining;

        /// <summary>
        /// Constructor.
        /// </summary>
        public LinearProjectileMovementProvider(int p_DeathDuration) : base(null)
        {
            // Set constraints
            m_DeathDuration = p_DeathDuration;
        }

        /// <summary>
        /// Reset.
        /// </summary>
        public override void DoReset()
        {
            this.UpdateState();
            m_DeathDurationRemaining = m_DeathDuration;
        }

        protected override void SetCandidateVelocity()
        {
            // Stop object when it dies
            if (!this.RoomObject.IsAlive)
            {
                m_Velocity.X = m_Velocity.Y = 0f;
            }
        }

        protected override void UpdateState(ref bool p_DoInteract)
        {
            if (this.RoomObject.IsAlive)
            {
                this.EvaluateSurroundings();
            }
            this.UpdateState();
        }

        void UpdateState()
        {
            // Update start
            if (this.RoomObject.IsAlive)
            {
                this.ActionState = (m_Velocity.X < 0 ? ActionStates.MoveLeft : ActionStates.MoveRight);
            }
            else
            {
                this.ActionState = ActionStates.Die;
                if (--m_DeathDurationRemaining == 0)
                {
                    this.RoomObject.FinishDeath();
                }
            }
        }

        #region Collision handlers

        protected override bool IsStopOnDeadlyCollision { get { return true; } }

        protected override void HandleObstacleCollision(Obstacle p_Obstacle, SurfaceDirections p_SurfaceDirection)
        {
            this.RoomObject.StartDeath();
        }

        protected override void HandleTileCollision(SurfaceDirections p_CollisionType)
        {
            this.RoomObject.StartDeath();
        }

        protected override void HandleTerrainCollision(Terrain p_Terrain)
        {
            this.RoomObject.StartDeath();
        }

        #endregion
    }

    #region Xml Reader

    /// <summary>
    /// Xml movement provider reader.
    /// </summary>
    public class XmlLinearProjectileMovementProviderReader : XmlRoomObjectMovementProviderReader
    {
        public XmlLinearProjectileMovementProviderReader()
        {
        }

        public override RoomObjectMovementProvider CreateInstance(OSATypes.TemplateMovementProvider p_Provider, OSATypes.TemplateMovementProvider p_RoomTemplateObject)
        {
            int deathDuration = p_RoomTemplateObject != null && int.TryParse(p_RoomTemplateObject.DeathDuration, out deathDuration) ? int.Parse(p_RoomTemplateObject.DeathDuration) : 120;
            return new LinearProjectileMovementProvider(deathDuration);
        }
    }

    #endregion
}
