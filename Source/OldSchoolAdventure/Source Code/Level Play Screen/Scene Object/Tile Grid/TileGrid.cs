using Destiny;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace OSA
{
    /// <summary>
    /// Tile grid class.
    /// </summary>
    public class TileGrid : SceneObject
    {
        public TileSet TileSet { get; private set; }
        public Size TileSize { get { return this.TileSet.TileSize; } }
        public Size GridSize { get; private set; }
        private Tile[,] m_Tiles;

        public Room Room { get; set; }

        public List<TileTemplate> TileTemplates { get { return this.TileSet.TileTemplates; } }
        public int MaxTileHeight { get { return this.TileSet.MaxHeight; } }
        public bool IsActive { get; private set; }
        public bool IsBackground { get; private set; }
        public bool IsFollowCamera { get { return !this.IsActive; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        public TileGrid(TileSet p_TileSet, Size p_GridSize, Vector2 p_Position, bool p_IsActive, bool p_IsBackground)
            : base(null, new Vector2(p_TileSet.TileSize.Width * p_GridSize.Width, p_TileSet.TileSize.Height * p_GridSize.Height), p_Position)
        {
            this.TileSet = p_TileSet;
            this.GridSize = p_GridSize;
            m_Tiles = new Tile[this.GridSize.Width, this.GridSize.Height];
            this.IsActive = p_IsActive;
            this.IsBackground = p_IsBackground;
        }

        /// <summary>
        /// Set the tile at the specified location.
        /// </summary>
        public void SetTile(int p_x, int p_y, char p_GridSymbol)
        {
            this.SetTile(p_x, p_y, this.TileSet.TileTemplateBySymbol(p_GridSymbol));
        }

        /// <summary>
        /// Set the tile at the specified location.
        /// </summary>
        public void SetTile(int p_x, int p_y, TileTemplate p_TileTemplate)
        {
            // Set tile
            m_Tiles[p_x, p_y] = (p_TileTemplate != null) ? new Tile(p_TileTemplate, new Point(p_x, p_y)) : null;

            // Update adjacent tile flags of this and adjacent tiles
            this.UpdateAdjacentTiles(p_x, p_y);
            this.UpdateAdjacentTiles(p_x - 1, p_y);
            this.UpdateAdjacentTiles(p_x, p_y - 1);
            this.UpdateAdjacentTiles(p_x + 1, p_y);
            this.UpdateAdjacentTiles(p_x, p_y + 1);
        }

        /// <summary>
        /// Delete the tile at the specified location.
        /// </summary>
        public void DeleteTile(int p_x, int p_y)
        {
            this.SetTile(p_x, p_y, null);
        }

        /// <summary>
        /// Update adjacent tiles.
        /// </summary>
        /// <param name="p_x">x location.</param>
        /// <param name="p_y">y location.</param>
        private void UpdateAdjacentTiles(int p_x, int p_y)
        {
            Tile tile = this.GetTileByGridPosition(p_x, p_y);
            if (tile != null)
            {
                tile.UpdateAdjacentTileFlags(this);
            }
        }

        /// <summary>
        /// Get the tile at the specified grid position.
        /// </summary>
        /// <param name="p_x">x location.</param>
        /// <param name="p_y">y location.</param>
        /// <returns>Tile at the specified grid position.</returns>
        public Tile GetTileByGridPosition(int p_x, int p_y)
        {
            Tile tile = null;
            int y = Math.Max(p_y, 0);
            if (p_x >= 0 && p_x < this.GridSize.Width && y < this.GridSize.Height)
            {
                tile = m_Tiles[p_x, y];
            }
            return tile;
        }

        /// <summary>
        /// Get the range of tiles between the specified start and end positions.
        /// </summary>
        public void GetGridXRange(float p_StartPosition, float p_EndPosition, out int p_GridStartIndex, out int p_GridEndIndex)
        {
            this.GetGridXRange(p_StartPosition, p_EndPosition, out p_GridStartIndex, out p_GridEndIndex, false);
        }

        /// <summary>
        /// Get the range of tiles between the specified start and end positions.
        /// </summary>
        /// <param name="p_TendOuter">If the position starts or ends at the boundary of 2 tiles, whether to include the outer tiles.</param>
        public void GetGridXRange(float p_StartPosition, float p_EndPosition, out int p_GridStartIndex, out int p_GridEndIndex, bool p_TendOuter)
        {
            p_GridStartIndex = this.GetGridXIndex(p_StartPosition, p_TendOuter);
            p_GridEndIndex = this.GetGridXIndex(p_EndPosition, !p_TendOuter);
        }

        /// <summary>
        /// Get the range of tiles between the specified start and end positions.
        /// </summary>
        public void GetGridYRange(float p_StartPosition, float p_EndPosition, out int p_GridStartIndex, out int p_GridEndIndex)
        {
            this.GetGridYRange(p_StartPosition, p_EndPosition, out p_GridStartIndex, out p_GridEndIndex, false);
        }

        /// <summary>
        /// Get the range of tiles between the specified start and end positions.
        /// </summary>
        /// <param name="p_TendOuter">If the position starts or ends at the boundary of 2 tiles, whether to include the outer tiles.</param>
        public void GetGridYRange(float p_StartPosition, float p_EndPosition, out int p_GridStartIndex, out int p_GridEndIndex, bool p_TendOuter)
        {
            p_GridStartIndex = this.GetGridYIndex(p_StartPosition, p_TendOuter);
            p_GridEndIndex = this.GetGridYIndex(p_EndPosition, !p_TendOuter);
        }

        /// <summary>
        /// Get the index of the specified position.
        /// </summary>
        /// <param name="p_StartPosition">Position.</param>
        /// <param name="p_IsTendLowest">Whether to tend to the lower index if exactly between 2 tiles.</param>
        /// <returns>Index of the specified position.</returns>
        public int GetGridXIndex(float p_Position, bool p_IsTendLowest)
        {
            float relativePosition = p_Position - this.Left;
            int index = (int)relativePosition / this.TileSize.Width;

            // If tend lowest, and end point is exactly at the start position of a new tile, return index of previous tile
            if (p_IsTendLowest && (Convert.ToSingle(index * this.TileSize.Width) == relativePosition))
            {
                index--;
            }
            return index;
        }

        /// <summary>
        /// Get the index of the specified position.
        /// </summary>
        /// <param name="p_StartPosition">Position.</param>
        /// <param name="p_IsTendLowest">Whether to tend to the lower index if exactly between 2 tiles.</param>
        /// <returns>Index of the specified position.</returns>
        public int GetGridYIndex(float p_Position, bool p_IsTendLowest)
        {
            float relativePosition = p_Position - this.Top;
            int index = (int)relativePosition / this.TileSize.Height;

            // If tend lowest, and end point is exactly at the start position of a new tile, return index of previous tile
            if (p_IsTendLowest && (Convert.ToSingle(index * this.TileSize.Height) == relativePosition))
            {
                index--;
            }
            return index;
        }
    }

    /// <summary>
    /// Tile grid list class.
    /// </summary>
    public class TileGridList : List<TileGrid>
    {
        /// <summary>
        /// Get the active tile grid.
        /// </summary>
        public TileGrid GetActive()
        {
            TileGrid result = null;
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].IsActive)
                {
                    result = this[i];
                    break;
                }
            }
            return result;
        }

        public void SetRoom(Room p_Room)
        {
            for (int i = 0; i < this.Count; i++)
            {
                this[i].Room = p_Room;
            }
        }
    }
}
