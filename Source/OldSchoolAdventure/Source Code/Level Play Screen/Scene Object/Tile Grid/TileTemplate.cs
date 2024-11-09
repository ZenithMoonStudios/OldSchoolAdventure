using Destiny;
using Microsoft.Xna.Framework;

namespace OSA
{
    /// <summary>
    /// Tile template class.
    /// </summary>
    public class TileTemplate
    {
        public string ObjectTypePath { get; private set; }
        public Size Size { get; private set; }

        public Vector2 Acceleration { get; private set; }
        public float Friction { get; private set; }
        public int Offense { get; private set; }
        public int Defense { get; private set; }
        public bool CompensateForGravityChanges { get; private set; }

        public SurfaceInformation LeftSide { get; private set; }
        public SurfaceInformation TopSide { get; private set; }
        public SurfaceInformation RightSide { get; private set; }
        public SurfaceInformation BottomSide { get; private set; }

        public bool IsLeftSolid { get { return this.LeftSide.IsSolid; } }
        public bool IsTopSolid { get { return this.TopSide.IsSolid; } }
        public bool IsRightSolid { get { return this.RightSide.IsSolid; } }
        public bool IsBottomSolid { get { return this.BottomSide.IsSolid; } }

        public int Height { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public TileTemplate(
            string p_Path,
            Size p_Size, Vector2 p_Acceleration, float p_Friction, int p_Offense, int p_Defense, bool p_CompensateForGravityChanges,
            SurfaceInformation p_LeftSide, SurfaceInformation p_TopSide, SurfaceInformation p_RightSide, SurfaceInformation p_BottomSide
            )
        {
            this.ObjectTypePath = p_Path;
            this.Size = p_Size;

            this.Acceleration = p_Acceleration;
            this.Friction = p_Friction;
            this.Offense = p_Offense;
            this.Defense = p_Defense;
            this.CompensateForGravityChanges = p_CompensateForGravityChanges;

            this.LeftSide = p_LeftSide;
            this.TopSide = p_TopSide;
            this.RightSide = p_RightSide;
            this.BottomSide = p_BottomSide;

            this.Height = 0;
        }
    }
}
