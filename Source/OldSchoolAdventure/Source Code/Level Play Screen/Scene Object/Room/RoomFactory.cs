using Destiny;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.IO;
using System.Xml.Serialization;

namespace OSA
{
    /// <summary>
    /// Room factory class.
    /// </summary>
    public static class RoomFactory
    {
        public delegate TileSet TileSetGetter(string p_Path);
        public delegate TerrainTemplate TerrainTemplateGetter(string p_Path);
        public delegate RoomObjectTemplate RoomObjectTemplateGetter(string p_Path);

        #region Load

        /// <summary>
        /// Load the room.
        /// </summary>
        /// <param name="p_LevelPath">Path to room.</param>
        /// <param name="p_ContentManager">Content manager.</param>
        /// <param name="p_GetTileSetDelegate">Method to get a tile set.</param>
        /// <param name="p_GetTerrainTemplateDelegate">Method to get a terrain template.</param>
        /// <param name="p_GetRoomObjectTemplateDelegate">Method to get a room object template.</param>
        /// <param name="p_RoomObjectProviderFactory">Room object provider factory.</param>
        /// <param name="p_GetCollisionProviderDelegate">Method to get a room object template.</param>
        /// <returns>Room.</returns>
        public static Room Load(
            string p_ObjectTypePath,
            ContentManager p_ContentManager,
            TileSetGetter p_GetTileSetDelegate,
            TerrainTemplateGetter p_GetTerrainTemplateDelegate,
            RoomObjectTemplateManager p_RoomObjectTemplateManager
            )
        {

            XmlSerializer serializer = new XmlSerializer(typeof(OSATypes.Room));
#if (MOBILE)
			Stream fs = TitleContainer.OpenStream(Path.Combine(p_ContentManager.RootDirectory, p_ObjectTypePath + ".xml"));
#else
            FileStream fs = new FileStream(Path.Combine(p_ContentManager.RootDirectory, p_ObjectTypePath + ".xml"), FileMode.Open);
#endif
            OSATypes.Room roomLoad;
            roomLoad = (OSATypes.Room)serializer.Deserialize(fs);
            OSATypes.RoomSize LoadSize = new OSATypes.RoomSize();
            OSATypes.RoomTileGrids LoadTiles = new OSATypes.RoomTileGrids();
            OSATypes.RoomLighting LoadLighting = null;
            OSATypes.RoomBackgrounds LoadBackground = null;

            foreach (object item in roomLoad.Items)
            {
                switch (item.GetType().ToString())
                {
                    case "OSATypes.RoomSize":
                        LoadSize = (OSATypes.RoomSize)item;
                        break;
                    case "OSATypes.RoomTileGrids":
                        LoadTiles = (OSATypes.RoomTileGrids)item;
                        break;
                    case "OSATypes.RoomLighting":
                        LoadLighting = (OSATypes.RoomLighting)item;
                        break;
                    case "OSATypes.RoomBackgrounds":
                        LoadBackground = (OSATypes.RoomBackgrounds)item;
                        break;
                }
            }

            string theme = "";
            float viscosityFactor = 1;
            Vector2 size = LoadSize != null ? OSATypes.Functions.LoadVector(LoadSize) : Vector2.Zero;

            // Load active tile grid and terrain
            TileGridList tileGrids = LoadTileGrids(LoadTiles, p_GetTileSetDelegate);
            TerrainList terrains = new TerrainList();
            TileGrid activeTileGrid = tileGrids.GetActive();

            // Load lighting information
            RoomLighting lighting = null;
            if (LoadLighting != null)
            {
                lighting = new RoomLighting();
                lighting.Axis = RoomLighting.AxisFromString(LoadLighting.Axis);

                // Get active tile grid properties for the relevant axis
                int tileGridStartPosition, tileSize, gridSize;
                if (lighting.Axis == RoomLighting.Axes.Y)
                {
                    tileGridStartPosition = Convert.ToInt32(activeTileGrid.Top);
                    tileSize = activeTileGrid.TileSize.Height;
                    gridSize = activeTileGrid.GridSize.Height;
                }
                else
                {
                    tileGridStartPosition = Convert.ToInt32(activeTileGrid.Left);
                    tileSize = activeTileGrid.TileSize.Width;
                    gridSize = activeTileGrid.GridSize.Width;
                }

                // Load lighting properties
                lighting.StartPosition = int.Parse(LoadLighting.Start[0].GridPosition) * tileSize + tileGridStartPosition;
                lighting.StartColor = new Color(int.Parse(LoadLighting.Start[0].Color[0].Red), int.Parse(LoadLighting.Start[0].Color[0].Green), int.Parse(LoadLighting.Start[0].Color[0].Blue), int.Parse(LoadLighting.Start[0].Color[0].Alpha));
                lighting.FinishPosition = int.Parse(LoadLighting.Finish[0].GridPosition) * tileSize + tileGridStartPosition;
                lighting.FinishColor = new Color(int.Parse(LoadLighting.Finish[0].Color[0].Red), int.Parse(LoadLighting.Finish[0].Color[0].Green), int.Parse(LoadLighting.Finish[0].Color[0].Blue), int.Parse(LoadLighting.Finish[0].Color[0].Alpha));
            }

            // Load background and foreground images
            Room.ScrollImageInfoList backgroundImages = LoadImages(LoadBackground);
            Room.ScrollImageInfoList foregroundImages = new Room.ScrollImageInfoList();

            // Create room
            Room room = new Room(p_ObjectTypePath, size, lighting, backgroundImages, foregroundImages, activeTileGrid, terrains, theme, viscosityFactor);
            tileGrids.SetRoom(room);

            // Load objects, and add them into room
            int objectCount = roomLoad.ContainerObjectTypesArray.Length;
            for (int i = 0; i < objectCount; i++)
            {
                if (roomLoad.Items[i] != null)
                    switch (roomLoad.ContainerObjectTypesArray[i])
                    {
                        case OSATypes.ContainerObjectTypes.Size:
                        case OSATypes.ContainerObjectTypes.Lighting:
                        case OSATypes.ContainerObjectTypes.Backgrounds:
                        case OSATypes.ContainerObjectTypes.TileGrids:
                        case OSATypes.ContainerObjectTypes.Enemies:
                            break;
                        case OSATypes.ContainerObjectTypes.Doors:
                            ProcessRoomObjects(new XmlDoorReader(), (OSATypes.RoomObject)roomLoad.Items[i], room, activeTileGrid, p_RoomObjectTemplateManager);
                            break;
                        case OSATypes.ContainerObjectTypes.Collectibles:
                            ProcessRoomObjects(new XmlCollectibleReader(), (OSATypes.RoomObject)roomLoad.Items[i], room, activeTileGrid, p_RoomObjectTemplateManager);
                            break;
                        case OSATypes.ContainerObjectTypes.Obstacles:
                            ProcessRoomObjects(new XmlObstacleReader(), (OSATypes.RoomObject)roomLoad.Items[i], room, activeTileGrid, p_RoomObjectTemplateManager);
                            break;
                        case OSATypes.ContainerObjectTypes.Directors:
                            ProcessRoomObjects(new XmlDirectorReader(), (OSATypes.RoomObject)roomLoad.Items[i], room, activeTileGrid, p_RoomObjectTemplateManager);
                            break;
                        case OSATypes.ContainerObjectTypes.SpawnPoints:
                            ProcessRoomObjects(new XmlSpawnPointReader(), (OSATypes.RoomObject)roomLoad.Items[i], room, activeTileGrid, p_RoomObjectTemplateManager);
                            break;
                        case OSATypes.ContainerObjectTypes.Speakers:
                            ProcessRoomObjects(new XmlSpeakerReader(), (OSATypes.RoomObject)roomLoad.Items[i], room, activeTileGrid, p_RoomObjectTemplateManager);
                            break;
                        case OSATypes.ContainerObjectTypes.MusicPoints:
                            ProcessRoomObjects(new XmlMusicPointReader(), (OSATypes.RoomObject)roomLoad.Items[i], room, activeTileGrid, p_RoomObjectTemplateManager);
                            break;
                        case OSATypes.ContainerObjectTypes.Switches:
                            ProcessRoomObjects(new XmlSwitchReader(), (OSATypes.RoomObject)roomLoad.Items[i], room, activeTileGrid, p_RoomObjectTemplateManager);
                            break;
                        default:
                            break;
                    }
            }


            return room;
        }

        #region Scroll images

        static Room.ScrollImageInfoList LoadImages(OSATypes.RoomBackgrounds backgrounds)
        {
            Room.ScrollImageInfoList images = new Room.ScrollImageInfoList();
            if (backgrounds != null)
            {

                foreach (OSATypes.RoomBackgroundsBackground background in backgrounds.Background)
                {
                    string path = background.Path;
                    Vector2 scrollRate = Vector2.Zero;
                    if (background.ScrollRate != null && background.ScrollRate.Count > 0)
                        scrollRate = new Vector2(float.Parse(background.ScrollRate[0].X), float.Parse(background.ScrollRate[0].Y));
                    images.Add(new Room.ScrollImageInfo(path, scrollRate));
                }
            }
            return images;
        }

        #endregion

        #region Tile grids

        /// <summary>
        /// Load tile grids.
        /// </summary>
        static TileGridList LoadTileGrids(OSATypes.RoomTileGrids grid, TileSetGetter p_GetTileSetDelegate)
        {
            TileGridList tileGrids = new TileGridList();
            if (grid != null)
            {
                foreach (OSATypes.RoomTileGridsTileGrid tile in grid.TileGrid)
                {
                    TileGrid tileGrid = LoadTileGrid(tile, p_GetTileSetDelegate);
                    tileGrids.Add(tileGrid);
                }
            }
            return tileGrids;
        }

        /// <summary>
        /// Load tile grid.
        /// </summary>
        static TileGrid LoadTileGrid(OSATypes.RoomTileGridsTileGrid tile, TileSetGetter p_GetTileSetDelegate)
        {
            bool isActive = tile.IsActive == "true" ? true : false;
            bool isBackground = false;
            Size gridSize = new Size(int.Parse(tile.GridSize[0].Width), int.Parse(tile.GridSize[0].Height));

            TileSet tileSet = p_GetTileSetDelegate(tile.TileSet);

            TileGrid tileGrid = new TileGrid(tileSet, gridSize, Vector2.Zero, isActive, isBackground);

            foreach (OSATypes.RoomTileGridsTileGridRows rows in tile.Rows)
                for (int rowIndex = 0; rowIndex < rows.Row.Count; rowIndex++)
                {
                    string tiles = rows.Row[rowIndex].Tiles;
                    for (int tileIndex = 0; tileIndex < tiles.Length; tileIndex++)
                    {
                        tileGrid.SetTile(tileIndex, rowIndex, tiles[tileIndex]);
                    }
                }

            return tileGrid;
        }

        #endregion

        //#region Terrains

        ///// <summary>
        ///// Load terrains.
        ///// </summary>
        //static TerrainList LoadTerrains(XmlNode p_XmlTerrainsNode, TerrainTemplateGetter p_GetTerrainTemplateDelegate)
        //{
        //    TerrainList terrains = new TerrainList();
        //    if (p_XmlTerrainsNode != null)
        //    {
        //        for (int terrainIndex = 0; terrainIndex < p_XmlTerrainsNode.ChildNodes.Count; terrainIndex++)
        //        {
        //            XmlNode xmlTerrainNode = p_XmlTerrainsNode.ChildNodes[terrainIndex];
        //            Terrain terrain = LoadTerrain(xmlTerrainNode, p_GetTerrainTemplateDelegate);
        //            terrains.Add(terrain);
        //        }
        //    }
        //    return terrains;
        //}

        ///// <summary>
        ///// Load terrain.
        ///// </summary>
        //static Terrain LoadTerrain(XmlNode p_XmlTerrainNode, TerrainTemplateGetter p_GetTerrainTemplateDelegate)
        //{
        //    int numberOfSegments = Convert.ToInt32(p_XmlTerrainNode.Attributes["NumberOfSegments"].Value);
        //    int startHeight = Convert.ToInt32(p_XmlTerrainNode.Attributes["StartHeight"].Value);
        //    string terrainTemplatePath = p_XmlTerrainNode.Attributes["Template"].Value;

        //    XmlNode xmlIncrementsNode = p_XmlTerrainNode["Increments"];
        //    TerrainSegmentList terrainSegments = new TerrainSegmentList();
        //    int segmentHeight = startHeight;
        //    for (int incrementIndex = 0; incrementIndex < xmlIncrementsNode.ChildNodes.Count; incrementIndex++)
        //    {
        //        XmlNode xmlIncrementNode = xmlIncrementsNode.ChildNodes[incrementIndex];
        //        int incrementSize = Convert.ToInt32(xmlIncrementNode.Attributes["Size"].Value);
        //        terrainSegments.Add(new TerrainSegment(segmentHeight, incrementSize));
        //        segmentHeight += incrementSize;
        //    }

        //    return new Terrain(p_GetTerrainTemplateDelegate(terrainTemplatePath), terrainSegments);
        //}

        //#endregion

        #region Room objects

        /// <summary>
        /// Process room objects.
        /// </summary>
        static void ProcessRoomObjects(XmlRoomObjectReader p_XmlReader, OSATypes.RoomObject objects, Room p_Room, TileGrid p_ActiveTileGrid, RoomObjectTemplateManager p_RoomObjectTemplateManager)
        {
            foreach (RoomObject obj in LoadRoomObjects(p_XmlReader, objects, p_ActiveTileGrid, p_RoomObjectTemplateManager))
            {
                p_Room.AddObject(obj);
            }
        }

        /// <summary>
        /// Load room objects.
        /// </summary>
        static RoomObjectList LoadRoomObjects(XmlRoomObjectReader p_XmlReader, OSATypes.RoomObject objects, TileGrid p_ActiveTileGrid, RoomObjectTemplateManager p_RoomObjectTemplateManager)
        {
            RoomObjectList roomObjects = new RoomObjectList();
            if (objects.RoomObjectItems != null)
                for (int i = 0; i < objects.RoomObjectItems.Length; i++)
                {
                    OSATypes.RoomObjectItem item = objects.RoomObjectItems[i];
                    RoomObject roomObject = LoadRoomObject(p_XmlReader, item, p_ActiveTileGrid, p_RoomObjectTemplateManager);
                    if (roomObject != null)
                    {
                        roomObjects.Add(roomObject);
                    }
                }
            return roomObjects;
        }

        /// <summary>
        /// Load room object.
        /// </summary>
        static RoomObject LoadRoomObject(XmlRoomObjectReader p_XmlReader, OSATypes.RoomObjectItem item, TileGrid p_ActiveTileGrid, RoomObjectTemplateManager p_RoomObjectTemplateManager)
        {
            return LoadRoomObject(p_XmlReader, item, p_ActiveTileGrid, p_RoomObjectTemplateManager, null);
        }

        /// <summary>
        /// Load room object.
        /// </summary>
        static RoomObject LoadRoomObject(XmlRoomObjectReader p_XmlReader, OSATypes.RoomObjectItem item, TileGrid p_ActiveTileGrid, RoomObjectTemplateManager p_RoomObjectTemplateManager, RoomObject p_ParentObject)
        {
            string templatePath = (p_ParentObject != null ? p_ParentObject.ShootInformation.ObjectPath : item.Template);
            RoomObjectTemplate template = p_RoomObjectTemplateManager.Get(templatePath);

            RoomObject roomObject = p_XmlReader.CreateInstance(templatePath, item, template.RoomTemplateObject, Vector2.Zero, p_ActiveTileGrid);
            if (roomObject.ShootInformation != null)
            {
                RoomObjectList projectiles = new RoomObjectList();
                for (int i = 0; i < roomObject.ShootInformation.ObjectMaxCount; i++)
                {
                    RoomObjectTemplate projectileTemplate = p_RoomObjectTemplateManager.Get(roomObject.ShootInformation.ObjectPath);
                    RoomObject projectile = LoadRoomObject(p_XmlReader, null, p_ActiveTileGrid, p_RoomObjectTemplateManager, roomObject);
                    projectiles.Add(projectile);
                }
                roomObject.BindProjectiles(projectiles);
            }
            return roomObject;
        }

        #endregion

        #endregion

    }
}
