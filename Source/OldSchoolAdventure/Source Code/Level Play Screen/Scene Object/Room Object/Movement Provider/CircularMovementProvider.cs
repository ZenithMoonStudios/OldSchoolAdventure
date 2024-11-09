using Microsoft.Xna.Framework;
using System;

namespace OSA
{
    /// <summary>
    /// Circular movement provider.
    /// </summary>
    public class CircularMovementProvider : RoomObjectMovementProvider
    {
        /// <summary>
        /// Direction enum.
        /// </summary>
        public enum Direction
        {
            Clockwise,
            AntiClockwise
        }

        /// <summary>
        /// Initialization.
        /// </summary>
        int m_TimePerRevolution;
        Vector2 m_RotationPointGridOffset = Vector2.Zero;
        float m_StartAngle;

        /// <summary>
        /// Private constraints.
        /// </summary>
        Vector2 m_RotationPoint = Vector2.Zero;
        Direction m_Direction;
        float m_AngleSpeed;
        float m_Radius;

        /// <summary>
        /// Current state.
        /// </summary>
        bool m_IsInitialized;
        float m_Angle;

        /// <summary>
        /// Constructor.
        /// </summary>
        public CircularMovementProvider(GameConditionList p_Conditions) : base(p_Conditions) { }

        /// <summary>
        /// Construct.
        /// </summary>
        public void Construct(int p_TimePerRevolution, Vector2 p_RotationPointGridOffset, Direction p_Direction)
        {
            // Set initialization variables
            m_TimePerRevolution = p_TimePerRevolution;
            m_RotationPointGridOffset = p_RotationPointGridOffset;
            m_StartAngle = (float)Math.Atan(-p_RotationPointGridOffset.Y / (float)-p_RotationPointGridOffset.X);
            if (p_RotationPointGridOffset.X > 0)
            {
                m_StartAngle += (float)Math.PI;
            }

            // Set constraints
            m_Direction = p_Direction;

            m_IsInitialized = false;
        }

        /// <summary>
        /// Reset.
        /// </summary>
        public override void DoReset()
        {
            m_Angle = m_StartAngle;
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        void Initialize()
        {
            m_Radius = (float)Math.Sqrt(
                Math.Pow(this.ActiveTileGridTileSize.Width * m_RotationPointGridOffset.X, 2) +
                Math.Pow(this.ActiveTileGridTileSize.Height * m_RotationPointGridOffset.Y, 2)
                );
            m_RotationPoint.X = m_Position.X + (this.ActiveTileGridTileSize.Width * m_RotationPointGridOffset.X);
            m_RotationPoint.Y = m_Position.Y + (this.ActiveTileGridTileSize.Height * m_RotationPointGridOffset.Y);

            m_AngleSpeed = (float)(2 * Math.PI / m_TimePerRevolution * this.Room.ViscosityFactor);

            m_IsInitialized = true;
        }

        /// <summary>
        /// Move.
        /// </summary>
        protected override void DoMove(ref bool p_DoInteract)
        {
            if (this.RoomObject.IsAlive)
            {
                // Cannot initialize until the room object has a valid position
                if (!m_IsInitialized)
                {
                    this.Initialize();
                }

                if (m_Radius > 0f)
                {
                    // Rotation in the appropriate direction
                    int targetFactor = (m_Direction == Direction.Clockwise) ? 1 : -1;
                    m_Angle += (targetFactor * m_AngleSpeed);

                    // Set new position
                    m_Position.X = m_RotationPoint.X + (float)(m_Radius * Math.Cos(m_Angle));
                    m_Position.Y = m_RotationPoint.Y + (float)(m_Radius * Math.Sin(m_Angle));
                }
            }
            else
            {
                // Do nothing
            }
        }
    }

    #region Xml Reader

    /// <summary>
    /// Xml circular movement provider p_Reader.
    /// </summary>
    public class XmlCircularMovementProviderReader : XmlRoomObjectMovementProviderReader
    {
        public XmlCircularMovementProviderReader()
        {
        }

        public override RoomObjectMovementProvider CreateInstance(OSATypes.TemplateMovementProvider p_Provider, OSATypes.TemplateMovementProvider p_TemplateRoomObject)
        {
            int timePerRevolution = int.TryParse(p_TemplateRoomObject.TimePerRevolution, out timePerRevolution) ? int.Parse(p_TemplateRoomObject.TimePerRevolution) : 300;

            // Direction can be defined in either the template or object
            string directionString = p_TemplateRoomObject.Direction != null ? p_TemplateRoomObject.Direction : string.Empty;
            CircularMovementProvider.Direction direction = (directionString == "AntiClockwise")
                ? CircularMovementProvider.Direction.AntiClockwise
                : CircularMovementProvider.Direction.Clockwise;

            // Rotation point can be defined in either the template or object
            //			XmlNode xmlRotationPointGridOffsetNode = null;
            Vector2 rotationPointGridOffset = Vector2.Zero;
            //if (p_Provider != null)
            //{
            //    xmlRotationPointGridOffsetNode = p_XmlObjectProviderNode["RotationPointGridOffset"];
            //    rotationPointGridOffset = p_Provider.rot XmlLoadHelper.LoadVector(xmlRotationPointGridOffsetNode);
            //}
            //if (xmlRotationPointGridOffsetNode == null)
            //{
            //    rotationPointGridOffset = p_TemplateRoomObject.RotationGridOffset != null && p_TemplateRoomObject.RotationGridOffset.Length > 0 ? OSATypes.Functions.LoadVector(p_TemplateRoomObject.RotationGridOffset[0].X, p_TemplateRoomObject.RotationGridOffset[0].Y) : Vector2.Zero;
            //}

            GameConditionList conditions = null;
            if (p_TemplateRoomObject != null && p_TemplateRoomObject.Conditions != null)
            {
                conditions = XmlGameConditionListReader.Load(p_TemplateRoomObject.Conditions);
            }

            CircularMovementProvider provider = new CircularMovementProvider(conditions);
            provider.Construct(timePerRevolution, rotationPointGridOffset, direction);
            return provider;
        }
    }

    #endregion
}
