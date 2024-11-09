using Destiny;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace OSA
{
    /// <summary>
    /// Draw manager class.
    /// </summary>
    public class DrawManager
    {
        Screen m_Screen;
        public ContentManager ContentManager { get { return m_Screen.ContentManager; } }
        public string ContentFolder { get; private set; }

        public SpriteBatch SpriteBatch { get { return m_Screen.SpriteBatch; } }

        // Store each object's state and the amount of time it has been in that state
        Dictionary<int, SceneObjectDrawStatus> m_SceneObjectGuidToDrawStatus = new Dictionary<int, SceneObjectDrawStatus>();

        // Sprite contents
        Dictionary<string, SceneObjectContent> m_SceneObjectTypePathToContent = new Dictionary<string, SceneObjectContent>();
        Dictionary<string, Texture2D> m_ContentPathToTexture = new Dictionary<string, Texture2D>();

        // Statics
        Rectangle m_WorldRegion = Rectangle.Empty;
        Rectangle m_ScreenRegion = Rectangle.Empty;

        // Temporary objects
        static Color s_DrawColor = Color.White;

        /// <summary>
        /// Constructor.
        /// </summary>
        public DrawManager(Screen p_Screen, string p_ContentFolder)
        {
            this.ContentFolder = p_ContentFolder;
            m_Screen = p_Screen;
        }

        #region Register

        /// <summary>
        /// Register a scene object for drawing.
        /// </summary>
        /// <param name="p_Door">Scene object.</param>
        public void Register(SceneObject p_SceneObject)
        {
            if (p_SceneObject is TileGrid)
            {
                // Load tile template sprites instead
                TileGrid tileGrid = p_SceneObject as TileGrid;
                foreach (TileTemplate tileTemplate in tileGrid.TileTemplates)
                {
                    this.LoadSprites(tileTemplate.ObjectTypePath);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(p_SceneObject.ObjectTypePath))
                {
                    this.RegisterType(p_SceneObject.ObjectTypePath);
                }
                m_SceneObjectGuidToDrawStatus.Add(p_SceneObject.ObjectGuid, new SceneObjectDrawStatus());
            }
        }

        /// <summary>
        /// Register object type.
        /// </summary>
        public void RegisterType(string p_ObjectTypePath)
        {
            this.LoadSprites(p_ObjectTypePath);
        }

        /// <summary>
        /// Load sprites.
        /// </summary>
        void LoadSprites(string p_ObjectTypePath)
        {
            // If no path is specified or path is already registered, return
            if (string.IsNullOrEmpty(p_ObjectTypePath) ||
                m_SceneObjectTypePathToContent.ContainsKey(p_ObjectTypePath))
            {
                return;
            }

            // Try to load scene object content first
            string contentPath = Path.Combine(this.ContentFolder, p_ObjectTypePath);
            SceneObjectContent objectContent = SceneObjectContentFactory.Load(contentPath, this.ContentManager, this.GetTexture);
            if (objectContent != null)
            {
                m_SceneObjectTypePathToContent.Add(p_ObjectTypePath, objectContent);
            }
        }

        /// <summary>
        /// Get the specified texture.
        /// </summary>
        Texture2D GetTexture(string p_ContentPath)
        {
            string contentPath = p_ContentPath;
            if (!contentPath.StartsWith(this.ContentFolder))
            {
                contentPath = Path.Combine(this.ContentFolder, p_ContentPath);
            }

            if (!m_ContentPathToTexture.ContainsKey(contentPath))
            {
                m_ContentPathToTexture.Add(contentPath, this.ContentManager.Load<Texture2D>(contentPath));
            }
            return m_ContentPathToTexture[contentPath];
        }

        #endregion

        #region Draw

        public void Draw(SceneObject p_SceneObject)
        {
            if (p_SceneObject is TileGrid)
            {
                this.DrawTileGrid(p_SceneObject as TileGrid, this.Camera);
            }
            else if (p_SceneObject is Terrain)
            {
                this.DrawTerrain(p_SceneObject as Terrain, this.Camera);
            }
            else if (p_SceneObject is RoomObject)
            {
                this.DrawRoomObject(p_SceneObject as RoomObject, this.Camera, string.Empty);
            }
        }

        void DrawTileGrid(TileGrid p_TileGrid, Camera p_Camera)
        {
            int gridStartXIndex;
            int gridStartYIndex;
            int gridEndXIndex;
            int gridEndYIndex;
            if (!p_TileGrid.IsFollowCamera)
            {
                gridStartXIndex = p_TileGrid.GetGridXIndex(p_Camera.WorldRegion.X, false) - 1;
                gridStartYIndex = p_TileGrid.GetGridYIndex(p_Camera.WorldRegion.Y, false) - 1;
                gridEndXIndex = p_TileGrid.GetGridXIndex(RectangleHelper.InnerRight(p_Camera.WorldRegion), false) + 1;
                gridEndYIndex = p_TileGrid.GetGridYIndex(RectangleHelper.InnerBottom(p_Camera.WorldRegion), false) + 1;
            }
            else
            {
                gridStartXIndex = p_TileGrid.GetGridXIndex(p_TileGrid.Left, false) - 1;
                gridStartYIndex = p_TileGrid.GetGridYIndex(p_TileGrid.Top, false) - 1;
                gridEndXIndex = p_TileGrid.GetGridXIndex(p_TileGrid.Right, false) + 1;
                gridEndYIndex = p_TileGrid.GetGridYIndex(p_TileGrid.Bottom, false) + 1;
            }

            // Determine color
            this.SetLightingDrawColor(p_Camera, p_TileGrid.Room, ref s_DrawColor);

            // Render tiles in order of smallest to greatest height
            for (int height = 0; height <= p_TileGrid.MaxTileHeight; height++)
            {
                // Iterate through visible tiles
                for (int x = gridStartXIndex; x < gridEndXIndex; x++)
                {
                    for (int y = gridStartYIndex; y < gridEndYIndex; y++)
                    {
                        Tile tile = p_TileGrid.GetTileByGridPosition(x, y);
                        if (tile != null && tile.Height == height)
                        {
#if !DEBUG
							if (tile.Path.Equals("Tile/Invisible"))
							{
								continue;
							}
#endif
                            // Draw tile
                            this.DrawTile(p_TileGrid, x, y, tile.Path, tile.AdjacentTileFlags, p_Camera, s_DrawColor);
                        }
                    }
                }
            }
        }

        void SetLightingDrawColor(Camera p_Camera, Room p_Room, ref Color p_Color)
        {
            RoomLighting lighting = p_Room.Lighting;
            bool doLighting = (lighting != null);

            // Turn off lighting model when editing
            ScreenList screens = m_Screen.ScreenManager.Screens;
            Screen topScreen = screens[screens.Count - 1];
            //if (topScreen is TileEditorScreen || topScreen is ObjectEditorScreen)
            //{
            //    doLighting = false;
            //}

            if (doLighting)
            {
                float positionPercent;
                float position = (lighting.Axis == RoomLighting.Axes.X)
                    ? (p_Camera.WorldRegion.X + (p_Camera.WorldRegion.Width / 2) - p_Room.Left)
                    : (p_Camera.WorldRegion.Y + (p_Camera.WorldRegion.Height / 2) - p_Room.Top);
                if (position <= lighting.StartPosition)
                {
                    positionPercent = 0f;
                }
                else if (position >= lighting.FinishPosition)
                {
                    positionPercent = 1f;
                }
                else
                {
                    positionPercent = (position - lighting.StartPosition) / (lighting.FinishPosition - lighting.StartPosition);
                }

                p_Color.R = (byte)MathHelper.Lerp(lighting.StartColor.R, lighting.FinishColor.R, positionPercent);
                p_Color.G = (byte)MathHelper.Lerp(lighting.StartColor.G, lighting.FinishColor.G, positionPercent);
                p_Color.B = (byte)MathHelper.Lerp(lighting.StartColor.B, lighting.FinishColor.B, positionPercent);
                p_Color.A = 255;
            }
            else
            {
                p_Color.R = p_Color.G = p_Color.B = p_Color.A = 255;
            }
        }

        public void DrawTile(TileGrid p_TileGrid, int p_GridXIndex, int p_GridYIndex, string p_TilePath, string p_ActionState, Camera p_Camera, Color p_Color)
        {
            Sprite sprite = this.GetSprite(p_TilePath, p_ActionState, m_Screen.ActiveFrames);

            int destinationX = !p_TileGrid.IsFollowCamera ? (int)p_TileGrid.Left : p_Camera.WorldRegion.X;
            int destinationY = !p_TileGrid.IsFollowCamera ? (int)p_TileGrid.Top : p_Camera.WorldRegion.Y;
            m_WorldRegion.X = destinationX + (p_GridXIndex * p_TileGrid.TileSize.Width) + sprite.DisplayOffset.Width;
            m_WorldRegion.Y = destinationY + (p_GridYIndex * p_TileGrid.TileSize.Height) + sprite.DisplayOffset.Height;
            m_WorldRegion.Width = sprite.TextureRegion.Width;
            m_WorldRegion.Height = sprite.TextureRegion.Height;

            p_Camera.WorldToScreen(m_WorldRegion, ref m_ScreenRegion);
            this.SpriteBatch.Draw(sprite.Texture, m_ScreenRegion, sprite.TextureRegion, p_Color);
        }

        void DrawTerrain(Terrain p_Terrain, Camera p_Camera)
        {
            // Work out which segments are visible, including an allowance for adjacent segments overlapping
            if (p_Terrain.TerrainPosition != TerrainPosition.None)
            {
                int startSegmentIndex = p_Terrain.GetSegmentIndex(p_Camera.WorldRegion.X) - 1;
                int endSegmentIndex = p_Terrain.GetSegmentIndex(RectangleHelper.InnerRight(p_Camera.WorldRegion)) + 1;

                // Iterate through visible segments
                for (int i = startSegmentIndex; i < endSegmentIndex; i++)
                {
                    TerrainSegment segment = p_Terrain.GetSegment(i);
                    if (segment != null)
                    {
                        // Draw segment
                        string animationName = segment.IncrementSize.ToString(CultureInfo.InvariantCulture);
                        Sprite sprite = this.GetSprite(p_Terrain.ObjectTypePath, animationName, m_Screen.ActiveFrames);

                        m_WorldRegion.X = (int)p_Terrain.Left + (i * p_Terrain.SegmentLength) + sprite.DisplayOffset.Width;
                        m_WorldRegion.Width = sprite.TextureRegion.Width;
                        m_WorldRegion.Height = sprite.TextureRegion.Height;
                        if (p_Terrain.TerrainPosition == TerrainPosition.Top)
                        {
                            m_WorldRegion.Y = (int)p_Terrain.Top + segment.StartHeight + sprite.DisplayOffset.Height;
                        }
                        else if (p_Terrain.TerrainPosition == TerrainPosition.Bottom)
                        {
                            m_WorldRegion.Y = (int)p_Terrain.Top - segment.StartHeight + sprite.DisplayOffset.Height;
                        }

                        p_Camera.WorldToScreen(m_WorldRegion, ref m_ScreenRegion);
                        this.SpriteBatch.Draw(sprite.Texture, m_ScreenRegion, sprite.TextureRegion, Color.White);
                    }
                }
            }
        }

        public void DrawRoomObject(RoomObject p_RoomObject, string p_OverridePath)
        {
            this.DrawRoomObject(p_RoomObject, this.Camera, p_OverridePath);
        }

        void DrawRoomObject(RoomObject p_RoomObject, Camera p_Camera, string p_OverridePath)
        {
            string path = string.IsNullOrEmpty(p_OverridePath) ? p_RoomObject.ObjectTypePath : p_OverridePath;

            // Get elapsed time in current state
            if (!string.IsNullOrEmpty(path))
            {
                // Determine action state
                SceneObjectDrawStatus drawStatus = m_SceneObjectGuidToDrawStatus[p_RoomObject.ObjectGuid];
                drawStatus.Update(p_RoomObject.ActionState, m_Screen.ActiveFrames);
                int actionElapsed = drawStatus.GetElapsed(m_Screen.ActiveFrames);

                // Draw object
                this.DrawRoomObject(
                    path, (int)p_RoomObject.Left, (int)p_RoomObject.Top, true,
                    p_RoomObject.ActionState, actionElapsed, p_Camera, p_RoomObject.Room, null
                    );
            }
        }

        public void DrawRoomObject(
            string p_ObjectTypePath, int p_PositionX, int p_PositionY, bool p_IsWorldPosition,
            string p_ActionState, int p_ActionElapsed, Camera p_Camera,
            Room p_Room, SpriteBatch p_SpriteBatch
            )
        {
            // Determine color
            this.SetLightingDrawColor(p_Camera, p_Room, ref s_DrawColor);

            // Draw object
            this.DrawObject(
                p_ObjectTypePath, p_PositionX, p_PositionY, p_IsWorldPosition,
                p_ActionState, p_ActionElapsed, p_Camera, s_DrawColor, p_SpriteBatch
                );
        }

        public void DrawObject(
            string p_ObjectTypePath, int p_PositionX, int p_PositionY, bool p_IsWorldPosition,
            string p_ActionState, int p_ActionElapsed, Camera p_Camera,
            Color p_Color, SpriteBatch p_SpriteBatch
            )
        {
            if (!string.IsNullOrEmpty(p_ObjectTypePath))
            {
#if !DEBUG
				if (
					p_ObjectTypePath.StartsWith("Mus") ||				// MusicPoint/
					p_ObjectTypePath.StartsWith("Dir") ||				// Director/
					p_ObjectTypePath.StartsWith("Spa") ||				// SpawnPoint/
					p_ObjectTypePath.StartsWith("Enemy/Solid") ||		// Enemy/SolidTileShooter
					p_ObjectTypePath.StartsWith("Collectible/Inv") ||	// Collectible/Invisible
					p_ObjectTypePath.StartsWith("Door/Inv") ||			// Door/Invisible
					p_ObjectTypePath.Equals("Speaker/AutoSpeak")
					)
				{
					return;
				}
#endif
                Sprite sprite = this.GetSprite(p_ObjectTypePath, p_ActionState, p_ActionElapsed);
                if (sprite != null)
                {
                    m_WorldRegion.X = p_PositionX + sprite.DisplayOffset.Width;
                    m_WorldRegion.Y = p_PositionY + sprite.DisplayOffset.Height;
                    m_WorldRegion.Width = sprite.TextureRegion.Width;
                    m_WorldRegion.Height = sprite.TextureRegion.Height;

                    SpriteBatch spriteBatch = p_SpriteBatch ?? this.SpriteBatch;
                    if (p_IsWorldPosition)
                    {
                        p_Camera.WorldToScreen(m_WorldRegion, ref m_ScreenRegion);
                        spriteBatch.Draw(sprite.Texture, m_ScreenRegion, sprite.TextureRegion, p_Color);
                    }
                    else
                    {
                        spriteBatch.Draw(sprite.Texture, m_WorldRegion, sprite.TextureRegion, p_Color);
                    }
                }
            }
        }

        /// <summary>
        /// Get sprite.
        /// </summary>
        Sprite GetSprite(string p_ObjectTypePath, string p_AnimationName, int p_ElapsedTime)
        {
            Sprite sprite = null;
            if (m_SceneObjectTypePathToContent.ContainsKey(p_ObjectTypePath))
            {
                SceneObjectContent content = m_SceneObjectTypePathToContent[p_ObjectTypePath];
                SpriteAnimation animation = content.GetSpriteAnimation(p_AnimationName);
                sprite = animation.GetSprite(p_ElapsedTime);
            }
            return sprite;
        }

        #endregion

        /// <summary>
        /// Camera.
        /// </summary>
        Camera Camera { get { return ((LevelPlayScreen)m_Screen).Camera; } }
    }
}
