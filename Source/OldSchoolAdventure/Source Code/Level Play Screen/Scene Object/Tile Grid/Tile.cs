using Microsoft.Xna.Framework;

namespace OSA
{
    /// <summary>
    /// Tile class.
    /// </summary>
    public class Tile
    {
        private const string c_DefaultAdjacentTileFlags = "0000";

        private TileTemplate m_Template;
        private Point m_GridPosition;

        public string Path { get { return m_Template.ObjectTypePath; } }

        public Vector2 Acceleration { get { return m_Template.Acceleration; } }
        public float Friction { get { return m_Template.Friction; } }
        public int Offense { get { return m_Template.Offense; } }
        public bool IsDeadly { get { return this.Offense > 0; } }
        public bool CompensateForGravityChanges { get { return m_Template.CompensateForGravityChanges; } }

        public SurfaceInformation LeftSide { get { return m_Template.LeftSide; } }
        public SurfaceInformation TopSide { get { return m_Template.TopSide; } }
        public SurfaceInformation RightSide { get { return m_Template.RightSide; } }
        public SurfaceInformation BottomSide { get { return m_Template.BottomSide; } }
        public bool IsLeftSolid { get { return m_Template.IsLeftSolid; } }
        public bool IsTopSolid { get { return m_Template.IsTopSolid; } }
        public bool IsRightSolid { get { return m_Template.IsRightSolid; } }
        public bool IsBottomSolid { get { return m_Template.IsBottomSolid; } }
        public int Height { get { return m_Template.Height; } }

        public string AdjacentTileFlags { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="p_Template">Tile template.</param>
        /// <param name="p_GridPosition">Position in the tile grid.</param>
        public Tile(TileTemplate p_Template, Point p_GridPosition)
        {
            m_Template = p_Template;
            m_GridPosition = p_GridPosition;
            this.AdjacentTileFlags = c_DefaultAdjacentTileFlags;
        }

        /// <summary>
        /// Update adjacent tile flags.
        /// </summary>
        /// <param name="p_TileGrid">Tile grid in which this tile resides.</param>
        public void UpdateAdjacentTileFlags(TileGrid p_TileGrid)
        {
            // Get adjacent tiles
            Tile leftTile = p_TileGrid.GetTileByGridPosition(m_GridPosition.X - 1, m_GridPosition.Y);
            Tile topTile = p_TileGrid.GetTileByGridPosition(m_GridPosition.X, m_GridPosition.Y - 1);
            Tile rightTile = p_TileGrid.GetTileByGridPosition(m_GridPosition.X + 1, m_GridPosition.Y);
            Tile bottomTile = p_TileGrid.GetTileByGridPosition(m_GridPosition.X, m_GridPosition.Y + 1);

            // Adjacent tile flag is 1 only if there is an adjacent tile of the same type
            this.AdjacentTileFlags = string.Format(
                "{0}{1}{2}{3}",
                IsSameHeight(this, leftTile) ? "1" : "0",
                IsSameHeight(this, topTile) ? "1" : "0",
                IsSameHeight(this, rightTile) ? "1" : "0",
                IsSameHeight(this, bottomTile) ? "1" : "0"
                );
        }

        /// <summary>
        /// Determine whether 2 tiles share the same template.
        /// </summary>
        /// <param name="p_Tile1">Tile 1.</param>
        /// <param name="p_Tile2">Tile 2.</param>
        /// <returns>Whether the 2 tiles share the same template.</returns>
        private static bool IsSameHeight(Tile p_Tile1, Tile p_Tile2)
        {
            return (p_Tile1 != null && p_Tile2 != null && p_Tile1.Height == p_Tile2.Height);
        }
    }
}
