using Destiny;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace OSA
{
    /// <summary>
    /// Room class.
    /// </summary>
    public class Room : SceneObject
    {
        public class ScrollImageInfo
        {
            public string Path;
            public Vector2 ScrollRate;
            public Texture2D Texture;
            public ScrollImageInfo(string p_Path, Vector2 p_ScrollRate)
            {
                this.Path = p_Path;
                this.ScrollRate = p_ScrollRate;
            }
        }
        public class ScrollImageInfoList : List<ScrollImageInfo> { }

        public RoomLighting Lighting { get; private set; }
        ScrollImageInfoList m_BackgroundImages = new ScrollImageInfoList();
        ScrollImageInfoList m_ForegroundImages = new ScrollImageInfoList();

        SceneObject m_BackgroundObjectsContainer = new SceneObject();
        SceneObject m_ForegroundObjectsContainer = new SceneObject();
        SceneObject m_MainCharacterContainer = new SceneObject();
        SceneObject m_TerrainsContainer = new SceneObject();

        SceneObject m_DoorsContainer = new SceneObject();
        SceneObject m_SwitchesContainer = new SceneObject();
        SceneObject m_CollectiblesContainer = new SceneObject();
        SceneObject m_SpeakersContainer = new SceneObject();
        SceneObject m_EnemiesContainer = new SceneObject();
        SceneObject m_ProjectilesContainer = new SceneObject();
        SceneObject m_ObstaclesContainer = new SceneObject();
        SceneObject m_DirectorsContainer = new SceneObject();
        SceneObject m_SpawnPointsContainer = new SceneObject();
        SceneObject m_MusicPointsContainer = new SceneObject();

        StringToRoomObjectDictionary m_NameToRoomObjects = new StringToRoomObjectDictionary();

        public string Theme { get; private set; }
        public float ViscosityFactor { get; private set; }

        public MainCharacter MainCharacter { get; private set; }
        public TileGrid ActiveTileGrid { get; private set; }

        public RoomObjectList Doors { get; private set; }
        public RoomObjectList Switches { get; private set; }
        public RoomObjectList Collectibles { get; private set; }
        public RoomObjectList Speakers { get; private set; }
        public RoomObjectList Obstacles { get; private set; }
        public RoomObjectList Directors { get; private set; }
        public RoomObjectList SpawnPoints { get; private set; }
        public RoomObjectList MusicPoints { get; private set; }
        public TerrainList Terrains { get; private set; }

        public Screen Screen { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Room(
            string p_ObjectTypePath,
            Vector2 p_Size,
            RoomLighting p_Lighting,
            ScrollImageInfoList p_BackgroundImages,
            ScrollImageInfoList p_ForegroundImages,
            TileGrid p_ActiveTileGrid,
            TerrainList p_Terrains,
            string p_Theme,
            float p_ViscosityFactor
            )
            : base(p_ObjectTypePath, p_Size, Vector2.Zero)
        {
            this.CanDraw = false;

            this.Lighting = p_Lighting;
            this.Theme = p_Theme;
            this.ViscosityFactor = p_ViscosityFactor;

            this.Doors = new RoomObjectList();
            this.Switches = new RoomObjectList();
            this.Collectibles = new RoomObjectList();
            this.Speakers = new RoomObjectList();
            this.Obstacles = new RoomObjectList();
            this.Directors = new RoomObjectList();
            this.SpawnPoints = new RoomObjectList();
            this.MusicPoints = new RoomObjectList();
            this.Terrains = new TerrainList();

            // Process tile grids
            this.ActiveTileGrid = p_ActiveTileGrid;

            // Setup scene hierarchy
            this.SceneObjects.Add(m_BackgroundObjectsContainer);
            this.SceneObjects.Add(m_ForegroundObjectsContainer);
            this.SceneObjects.Add(m_TerrainsContainer);
            this.SceneObjects.Add(m_MainCharacterContainer);
            m_BackgroundObjectsContainer.SceneObjects.Add(m_DoorsContainer);
            m_BackgroundObjectsContainer.SceneObjects.Add(m_ProjectilesContainer);
            m_BackgroundObjectsContainer.SceneObjects.Add(m_SwitchesContainer);
            m_BackgroundObjectsContainer.SceneObjects.Add(m_CollectiblesContainer);
            m_BackgroundObjectsContainer.SceneObjects.Add(m_SpeakersContainer);
            m_ForegroundObjectsContainer.SceneObjects.Add(m_EnemiesContainer);
            m_ForegroundObjectsContainer.SceneObjects.Add(m_ObstaclesContainer);
            m_ForegroundObjectsContainer.SceneObjects.Add(this.ActiveTileGrid);
            m_ForegroundObjectsContainer.SceneObjects.Add(m_SpawnPointsContainer);
            m_ForegroundObjectsContainer.SceneObjects.Add(m_MusicPointsContainer);
            m_ForegroundObjectsContainer.SceneObjects.Add(m_DirectorsContainer);

            // Initialize terrains
            foreach (Terrain terrain in p_Terrains)
            {
                terrain.Dock(this);
                this.Terrains.Add(terrain);
                m_TerrainsContainer.SceneObjects.Add(terrain);
            }

            // Initialize images
            foreach (ScrollImageInfo image in p_BackgroundImages) { m_BackgroundImages.Add(image); }
            foreach (ScrollImageInfo image in p_ForegroundImages) { m_ForegroundImages.Add(image); }
        }

        int m_NextId = 1;
        public string GetRepeatableId()
        {
            return string.Format("{0}/{1}", this.ObjectTypePath, m_NextId++.ToString(CultureInfo.InvariantCulture));
        }

        #region Add and Remove Objects

        public void AddObject(RoomObject p_RoomObject)
        {
            if (p_RoomObject is SpawnPoint)
            {
                this.SpawnPoints.Add(p_RoomObject);
                m_SpawnPointsContainer.SceneObjects.Add(p_RoomObject);
            }
            else if (p_RoomObject is Door)
            {
                this.Doors.Add(p_RoomObject);
                m_DoorsContainer.SceneObjects.Add(p_RoomObject);
            }
            else if (p_RoomObject is Switch)
            {
                this.Switches.Add(p_RoomObject);
                m_SwitchesContainer.SceneObjects.Add(p_RoomObject);
            }
            else if (p_RoomObject is Collectible)
            {
                this.Collectibles.Add(p_RoomObject);
                m_CollectiblesContainer.SceneObjects.Add(p_RoomObject);
            }
            else if (p_RoomObject is Speaker)
            {
                this.Speakers.Add(p_RoomObject);
                m_SpeakersContainer.SceneObjects.Add(p_RoomObject);
            }
            else if (p_RoomObject is Obstacle)
            {
                this.Obstacles.Add(p_RoomObject);
                if (p_RoomObject.ObjectTypePath.StartsWith("Enemy"))
                {
                    m_EnemiesContainer.SceneObjects.Add(p_RoomObject);
                }
                else if (p_RoomObject.ObjectTypePath.StartsWith("Projectile"))
                {
                    m_ProjectilesContainer.SceneObjects.Add(p_RoomObject);
                }
                else
                {
                    m_ObstaclesContainer.SceneObjects.Add(p_RoomObject);
                }
            }
            else if (p_RoomObject is Director)
            {
                this.Directors.Add(p_RoomObject);
                m_DirectorsContainer.SceneObjects.Add(p_RoomObject);
            }
            else if (p_RoomObject is MusicPoint)
            {
                this.MusicPoints.Add(p_RoomObject);
                m_MusicPointsContainer.SceneObjects.Add(p_RoomObject);
            }

            p_RoomObject.BindToRoom(this);
            if (this.MainCharacter != null)
            {
                p_RoomObject.ResetAndChildren();
            }
            if (!string.IsNullOrEmpty(p_RoomObject.Name))
            {
                m_NameToRoomObjects.Add(p_RoomObject.Name, p_RoomObject);
            }
        }

        public void RemoveObject(RoomObject p_RoomObject)
        {
            if (p_RoomObject is SpawnPoint)
            {
                this.SpawnPoints.Remove(p_RoomObject);
                m_SpawnPointsContainer.SceneObjects.Remove(p_RoomObject);
            }
            else if (p_RoomObject is Door)
            {
                this.Doors.Remove(p_RoomObject);
                m_DoorsContainer.SceneObjects.Remove(p_RoomObject);
            }
            else if (p_RoomObject is Switch)
            {
                this.Switches.Remove(p_RoomObject);
                m_SwitchesContainer.SceneObjects.Remove(p_RoomObject);
            }
            else if (p_RoomObject is Collectible)
            {
                this.Collectibles.Remove(p_RoomObject);
                m_CollectiblesContainer.SceneObjects.Remove(p_RoomObject);
            }
            else if (p_RoomObject is Speaker)
            {
                this.Speakers.Remove(p_RoomObject);
                m_SpeakersContainer.SceneObjects.Remove(p_RoomObject);
            }
            else if (p_RoomObject is Obstacle)
            {
                this.Obstacles.Remove(p_RoomObject);
                if (p_RoomObject.ObjectTypePath.StartsWith("Enemy"))
                {
                    m_EnemiesContainer.SceneObjects.Remove(p_RoomObject);
                }
                else if (p_RoomObject.ObjectTypePath.StartsWith("Projectile"))
                {
                    m_ProjectilesContainer.SceneObjects.Remove(p_RoomObject);
                }
                else
                {
                    m_ObstaclesContainer.SceneObjects.Remove(p_RoomObject);
                }
            }
            else if (p_RoomObject is Director)
            {
                this.Directors.Remove(p_RoomObject);
                m_DirectorsContainer.SceneObjects.Remove(p_RoomObject);
            }
            else if (p_RoomObject is MusicPoint)
            {
                this.MusicPoints.Remove(p_RoomObject);
                m_MusicPointsContainer.SceneObjects.Remove(p_RoomObject);
            }

            if (!string.IsNullOrEmpty(p_RoomObject.Name))
            {
                m_NameToRoomObjects.Remove(p_RoomObject.Name);
            }
        }

        #endregion

        #region Load and unload content

        protected override void LoadContent(DrawManager p_DrawManager)
        {
            this.LoadAndFillImageTextures(p_DrawManager, m_BackgroundImages);
            this.LoadAndFillImageTextures(p_DrawManager, m_ForegroundImages);
        }

        void LoadAndFillImageTextures(DrawManager p_DrawManager, ScrollImageInfoList p_Images)
        {
            foreach (ScrollImageInfo image in p_Images)
            {
                image.Texture = p_DrawManager.ContentManager.Load<Texture2D>(
                    Path.Combine(p_DrawManager.ContentFolder, image.Path)
                    );
            }
        }

        #endregion

        /// <summary>
        /// Enter.
        /// </summary>
        /// <param name="p_MainCharacter">Main character.</param>
        public void Enter(MainCharacter p_MainCharacter)
        {
            // Add main character to room
            this.MainCharacter = p_MainCharacter;
            m_MainCharacterContainer.SceneObjects.Clear();
            m_MainCharacterContainer.SceneObjects.Add(this.MainCharacter);

            // Revert room objects back to their initial position and state
            this.ResetAndChildren();
        }

        /// <summary>
        /// Get the specified room object by name.
        /// </summary>
        public RoomObject GetNamedRoomObject(string p_Name)
        {
            return m_NameToRoomObjects[p_Name];
        }

        #region Draw

        public override void DrawAndChildren(DrawManager p_DrawManager)
        {
            this.DrawImages(m_BackgroundImages, p_DrawManager);

            m_BackgroundObjectsContainer.DrawAndChildren(p_DrawManager);
            m_MainCharacterContainer.DrawAndChildren(p_DrawManager);
            m_ForegroundObjectsContainer.DrawAndChildren(p_DrawManager);
            m_TerrainsContainer.DrawAndChildren(p_DrawManager);

            this.DrawImages(m_ForegroundImages, p_DrawManager);
        }

        static Vector2 s_DrawVector;
        void DrawImages(ScrollImageInfoList p_Images, DrawManager p_DrawManager)
        {
            for (int i = 0; i < p_Images.Count; i++)
            {
                ScrollImageInfo image = p_Images[i];

                // Determine scroll position
                int scrollCameraX = (int)(this.Camera.WorldRegion.X * image.ScrollRate.X);
                int scrollCameraY = (int)(this.Camera.WorldRegion.Y * image.ScrollRate.Y);
                int startX = scrollCameraX / image.Texture.Width;
                int startY = scrollCameraY / image.Texture.Height;
                int endX = (scrollCameraX + this.Camera.WorldRegion.Width) / image.Texture.Width;
                int endY = (scrollCameraY + this.Camera.WorldRegion.Height) / image.Texture.Height;

                // Draw images
                for (int x = startX; x <= endX; x++)
                {
                    s_DrawVector.X = (x * image.Texture.Width) - scrollCameraX;
                    for (int y = startY; y <= endY; y++)
                    {
                        s_DrawVector.Y = (y * image.Texture.Height) - scrollCameraY;
                        p_DrawManager.SpriteBatch.Draw(image.Texture, s_DrawVector, Color.White);
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// Camera.
        /// </summary>
        Camera Camera { get { return ((LevelPlayScreen)this.Screen).Camera; } }
    }
}
