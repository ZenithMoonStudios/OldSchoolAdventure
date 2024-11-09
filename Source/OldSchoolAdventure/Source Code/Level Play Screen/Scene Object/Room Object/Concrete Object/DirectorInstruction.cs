using Microsoft.Xna.Framework;

namespace OSA
{
    /// <summary>
    /// Director instruction class.
    /// </summary>
    public class DirectorInstruction
    {
        public enum Types { Linear, Circular }

        public Types Type { get; private set; }

        public Vector2 LinearVelocity { get; private set; }

        public bool CircularIsClockwise { get; private set; }
        public int CircularFramesPerRevolution { get; private set; }
        public Vector2 CircularRotationGridOffset { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public DirectorInstruction(Vector2 p_Velocity)
        {
            this.Type = Types.Linear;
            this.LinearVelocity = p_Velocity;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public DirectorInstruction(bool p_IsClockwise, int p_FramesPerRevolution, Vector2 p_RotationGridOffset)
        {
            this.Type = Types.Circular;
            this.CircularIsClockwise = p_IsClockwise;
            this.CircularFramesPerRevolution = p_FramesPerRevolution;
            this.CircularRotationGridOffset = p_RotationGridOffset;
        }
    }
}
