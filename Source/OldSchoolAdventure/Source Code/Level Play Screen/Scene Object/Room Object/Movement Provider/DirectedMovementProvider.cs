using Microsoft.Xna.Framework;

namespace OSA
{
    /// <summary>
    /// Directed movement provider.
    /// </summary>
    public class DirectedMovementProvider : RoomObjectMovementProvider
    {
        CircularMovementProvider m_CircularMovementProvider = null;
        DirectorInstruction m_LastInstruction = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        public DirectedMovementProvider(GameConditionList p_Conditions) : base(p_Conditions)
        {
            m_CircularMovementProvider = new CircularMovementProvider(p_Conditions);
        }

        /// <summary>
        /// Reset.
        /// </summary>
        public override void DoReset()
        {
            m_LastInstruction = null;
            this.UpdateActionStateString();
        }

        /// <summary>
        /// Move.
        /// </summary>
        protected override void DoMove(ref bool p_DoInteract)
        {
            if (this.RoomObject.IsAlive)
            {
                DirectorInstruction instruction = this.RoomObject.LastDirectorInstruction;

                if (instruction != m_LastInstruction)
                {
                    // Don't update director if going from one circular path to another
                    if (instruction == null ||
                        m_LastInstruction == null ||
                        instruction.Type != m_LastInstruction.Type ||
                        instruction.Type != DirectorInstruction.Types.Circular
                        )
                    {
                        // Update last instruction
                        m_LastInstruction = instruction;
                        if (m_LastInstruction.Type == DirectorInstruction.Types.Circular)
                        {
                            m_CircularMovementProvider.Construct(
                                m_LastInstruction.CircularFramesPerRevolution,
                                m_LastInstruction.CircularRotationGridOffset,
                                m_LastInstruction.CircularIsClockwise ? CircularMovementProvider.Direction.Clockwise : CircularMovementProvider.Direction.AntiClockwise
                                );
                            m_CircularMovementProvider.Initialize(this.RoomObject);
                            m_CircularMovementProvider.Reset(Vector2.Zero, Vector2.Zero);
                        }
                    }
                }
                if (m_LastInstruction != null)
                {
                    if (m_LastInstruction.Type == DirectorInstruction.Types.Linear)
                    {
                        m_Position.X += m_LastInstruction.LinearVelocity.X;
                        m_Position.Y += m_LastInstruction.LinearVelocity.Y;
                    }
                    else if (m_LastInstruction.Type == DirectorInstruction.Types.Circular)
                    {
                        m_CircularMovementProvider.Move(ref m_Position, m_Velocity, ref p_DoInteract);
                    }
                }
            }
            else
            {
                // Do nothing
            }
        }

        protected override void UpdateState(ref bool p_DoInteract)
        {
            this.UpdateActionStateString();
        }

        void UpdateActionStateString()
        {
            this.ActionState = this.RoomObject.IsShooting ? "Shoot" : "Default";
        }
    }

    #region Xml Reader

    /// <summary>
    /// Xml directed movement provider p_Reader.
    /// </summary>
    public class XmlDirectedMovementProviderReader : XmlRoomObjectMovementProviderReader
    {
        public XmlDirectedMovementProviderReader()
        {
        }

        public override RoomObjectMovementProvider CreateInstance(OSATypes.TemplateMovementProvider p_Provider, OSATypes.TemplateMovementProvider p_TemplateRoomObject)
        {
            GameConditionList conditions = null;
            if (p_Provider != null)
            {
                conditions = XmlGameConditionListReader.Load(p_Provider.Conditions);
            }

            return new DirectedMovementProvider(conditions);
        }
    }

    #endregion
}
