namespace OSA
{
    /// <summary>
    /// Terrain position helper class.
    /// </summary>
    public static class TerrainPositionHelper
    {
        /// <summary>
        /// Convert a string to a terrain position.
        /// </summary>
        /// <param name="p_PositionString">Terrain position string.</param>
        /// <returns>Terrain position.</returns>
        public static TerrainPosition FromString(string p_PositionString)
        {
            TerrainPosition position = TerrainPosition.None;
            switch (p_PositionString)
            {
                case "Top": position = TerrainPosition.Top; break;
                case "Bottom": position = TerrainPosition.Bottom; break;
            }
            return position;
        }
    }
}
