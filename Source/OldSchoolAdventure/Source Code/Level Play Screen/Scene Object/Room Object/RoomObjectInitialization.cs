using Destiny;
using Microsoft.Xna.Framework;

namespace OSA
{
    /// <summary>
    /// Room object initialization class.
    /// </summary>
    public class RoomObjectInitialization
    {
        public string ObjectTypePath { get; private set; }
        public Vector2 Size { get; private set; }
        public Vector2 Position { get; private set; }
        public Vector2 GridPosition { get; private set; }
        public Vector2 Velocity { get; private set; }
        public string Name { get; private set; }
        public RoomObjectMovementProvider MovementProvider { get; private set; }
        public VerticalAlignment VerticalAlignment { get; private set; }
        public string RenderMode { get; private set; }
        public OSATypes.TemplateMovementProvider ObjectMovementProvider { get; private set; }
        public ShootInformation ShootInformation { get; private set; }

        public int Offense { get; set; }
        public int Defense { get; set; }

        public StringToStringDictionary AudioCues { get; private set; }
        public GameConditionList Conditions { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public RoomObjectInitialization(
            string p_ObjectTypePath,
            Vector2 p_Size,
            Vector2 p_Position,
            Vector2 p_GridPosition,
            Vector2 p_Velocity,
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
            )
        {
            this.ObjectTypePath = p_ObjectTypePath;
            this.Size = p_Size;
            this.Position = p_Position;
            this.GridPosition = p_GridPosition;
            this.Velocity = p_Velocity;
            this.Name = p_Name;
            this.MovementProvider = p_MovementProvider;
            this.VerticalAlignment = p_VerticalAlignment;
            this.RenderMode = p_RenderMode;
            this.ObjectMovementProvider = p_ObjectMovementProvider;
            this.ShootInformation = p_ShootInformation;
            this.Offense = p_Offense;
            this.Defense = p_Defense;
            this.AudioCues = p_AudioCues;
            this.Conditions = p_Conditions;
        }
    }
}
