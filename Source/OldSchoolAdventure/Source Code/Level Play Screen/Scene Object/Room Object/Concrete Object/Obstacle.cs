using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace OSA
{
    /// <summary>
    /// Obstacle class.
    /// </summary>
    public class Obstacle : RoomObject
    {
        public float Friction { get; private set; }

        public bool IsDeadly { get { return this.Offense > 0; } }

        public SurfaceInformation LeftSide { get; private set; }
        public SurfaceInformation TopSide { get; private set; }
        public SurfaceInformation RightSide { get; private set; }
        public SurfaceInformation BottomSide { get; private set; }

        public bool IsLeftSolid { get { return this.LeftSide.IsSolid; } }
        public bool IsTopSolid { get { return this.TopSide.IsSolid; } }
        public bool IsRightSolid { get { return this.RightSide.IsSolid; } }
        public bool IsBottomSolid { get { return this.BottomSide.IsSolid; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Obstacle(
            float p_Friction,
            SurfaceInformation p_LeftSide, SurfaceInformation p_TopSide, SurfaceInformation p_RightSide, SurfaceInformation p_BottomSide,
            RoomObjectInitialization p_Init
            )
            : base(p_Init)
        {
            this.Friction = p_Friction;
            this.LeftSide = p_LeftSide;
            this.TopSide = p_TopSide;
            this.RightSide = p_RightSide;
            this.BottomSide = p_BottomSide;
        }

        public SurfaceInformation GetSide(SurfaceDirections p_Direction)
        {
            SurfaceInformation result = null;
            switch (p_Direction)
            {
                case SurfaceDirections.Left: result = this.LeftSide; break;
                case SurfaceDirections.Right: result = this.RightSide; break;
                case SurfaceDirections.Top: result = this.TopSide; break;
                case SurfaceDirections.Bottom: result = this.BottomSide; break;
            }
            return result;
        }
    }

    /// <summary>
    /// Obstacle list class.
    /// </summary>
    public class ObstacleList : List<Obstacle> { }

    /// <summary>
    /// String to obstacle dictionary class.
    /// </summary>
    public class StringToObstacleDictionary : Dictionary<string, Obstacle> { }

    /// <summary>
    /// Xml reader.
    /// </summary>
    public class XmlObstacleReader : XmlRoomObjectReader
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public XmlObstacleReader()
        {
        }

        /// <summary>
        /// Create an instance.
        /// </summary>
        public override RoomObject CreateInstance(string p_ObjectTypePath, OSATypes.RoomObjectItem item, OSATypes.RoomObjectTemplate p_RoomTemplateObject, Vector2 p_PreferredGridPosition, TileGrid p_TileGrid)
        {
            float friction = float.TryParse(p_RoomTemplateObject.Friction, out friction) ? float.Parse(p_RoomTemplateObject.Friction) : 0.25f;
            bool isDeadly = bool.TryParse(p_RoomTemplateObject.IsDeadly, out isDeadly) ? bool.Parse(p_RoomTemplateObject.IsDeadly) : false;

            RoomObjectInitialization initializationInfo = GetInitializationInfo(p_ObjectTypePath, item, p_RoomTemplateObject, p_PreferredGridPosition, p_TileGrid);
            if (isDeadly && initializationInfo.Offense == 0)
            {
                initializationInfo.Offense = 1;
            }

            SurfaceInformation leftSurface = SurfaceInformation.Load(SurfaceDirections.Left, p_RoomTemplateObject.Left[0], friction, initializationInfo.Offense, initializationInfo.Defense);
            SurfaceInformation topSurface = SurfaceInformation.Load(SurfaceDirections.Top, p_RoomTemplateObject.Top[0], friction, initializationInfo.Offense, initializationInfo.Defense);
            SurfaceInformation rightSurface = SurfaceInformation.Load(SurfaceDirections.Right, p_RoomTemplateObject.Right[0], friction, initializationInfo.Offense, initializationInfo.Defense);
            SurfaceInformation bottomSurface = SurfaceInformation.Load(SurfaceDirections.Bottom, p_RoomTemplateObject.Bottom[0], friction, initializationInfo.Offense, initializationInfo.Defense);

            return new Obstacle(friction, leftSurface, topSurface, rightSurface, bottomSurface, initializationInfo);
        }
    }

}
