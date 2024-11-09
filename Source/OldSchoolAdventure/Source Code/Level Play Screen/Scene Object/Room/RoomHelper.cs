using Microsoft.Xna.Framework;

namespace OSA
{
    public static class RoomHelper
    {
        /// <summary>
        /// Get object position from grid position.
        /// </summary>
        static Vector2 s_TempVector;
        public static Vector2 GetPositionFromGridPosition(
            Vector2 p_Size, VerticalAlignment p_VerticalAlignment, Vector2 p_GridPosition, TileGrid p_TileGrid
            )
        {
            GetPositionFromGridPosition(p_Size, p_VerticalAlignment, p_GridPosition, p_TileGrid, ref s_TempVector);
            return s_TempVector;
        }

        public static void GetPositionFromGridPosition(
            Vector2 p_Size, VerticalAlignment p_VerticalAlignment, Vector2 p_GridPosition, TileGrid p_TileGrid,
            ref Vector2 p_Result
        )
        {
            // Convert grid position to position using vertical alignment type
            int verticalPosition = 0;
            if (p_VerticalAlignment == VerticalAlignment.Top)
            {
                verticalPosition = (int)(p_TileGrid.Top + (p_GridPosition.Y * p_TileGrid.TileSize.Height));
            }
            else if (p_VerticalAlignment == VerticalAlignment.Bottom)
            {
                verticalPosition = (int)(p_TileGrid.Top + ((p_GridPosition.Y + 1) * p_TileGrid.TileSize.Height) - p_Size.Y);
            }
            else
            {
                verticalPosition = (int)(p_TileGrid.Top + ((p_GridPosition.Y + 0.5f) * p_TileGrid.TileSize.Height) - (p_Size.Y / 2f));
            }

            // Set result
            p_Result.X = p_TileGrid.Left + ((p_GridPosition.X + 0.5f) * p_TileGrid.TileSize.Width) - (p_Size.X / 2f);
            p_Result.Y = verticalPosition;
        }
    }
}
