using System.Collections.Generic;

namespace OSA
{
    /// <summary>
    /// Terrain segment class.
    /// </summary>
    public class TerrainSegment
    {
        public int StartHeight { get; private set; }
        public int IncrementSize { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public TerrainSegment(int p_StartHeight, int p_IncrementSize)
        {
            this.StartHeight = p_StartHeight;
            this.IncrementSize = p_IncrementSize;
        }
    }

    /// <summary>
    /// Terrain segment list class.
    /// </summary>
    public class TerrainSegmentList : List<TerrainSegment> { }
}
