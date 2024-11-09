using Destiny;

namespace OSA
{
    /// <summary>
    /// Terrain template class.
    /// </summary>
    public class TerrainTemplate
    {
        public string ObjectTypePath { get; private set; }
        public TerrainPosition TerrainPosition { get; private set; }
        public int SegmentLength { get; private set; }
        public float Friction { get; private set; }

        private IntToBoolDictionary m_AllowedIncrements = new IntToBoolDictionary();

        /// <summary>
        /// Constructor.
        /// </summary>
        public TerrainTemplate(string p_Path, TerrainPosition p_TerrainPosition, int p_SegmentLength, float p_Friction, IntList p_AllowedIncrements)
        {
            this.ObjectTypePath = p_Path;
            this.TerrainPosition = p_TerrainPosition;
            this.SegmentLength = p_SegmentLength;
            this.Friction = p_Friction;

            for (int i = 0; i < p_AllowedIncrements.Count; i++)
            {
                m_AllowedIncrements.Add(p_AllowedIncrements[i], true);
            }
        }
    }
}
