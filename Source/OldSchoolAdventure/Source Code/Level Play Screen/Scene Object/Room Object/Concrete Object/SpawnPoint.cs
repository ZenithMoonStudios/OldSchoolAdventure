using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace OSA
{
    /// <summary>
    /// Spawn point class.
    /// </summary>
    public class SpawnPoint : Door
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public SpawnPoint(RoomObjectInitialization p_Init)
            : base(null, DoorLockType.None, null, false, null, true, true, p_Init)
        {
        }

        protected override void HandleCollision(SceneObject p_Object, bool p_IsCollision)
        {
            if (this.Target.IsEmpty)
            {
                this.Target.Set(this.Room.ObjectTypePath, this.Name);
            }
            base.HandleCollision(p_Object, p_IsCollision);
        }
    }

    public class SpawnPointList : List<SpawnPoint> { }
    public class StringToSpawnPointDictionary : Dictionary<string, SpawnPoint> { }

    /// <summary>
    /// Xml reader.
    /// </summary>
    public class XmlSpawnPointReader : XmlRoomObjectReader
    {
        public XmlSpawnPointReader() { }

        public override RoomObject CreateInstance(string p_ObjectTypePath, OSATypes.RoomObjectItem item, OSATypes.RoomObjectTemplate p_RoomTemplateObject, Vector2 p_PreferredGridPosition, TileGrid p_TileGrid)
        {
            RoomObjectInitialization initializationInfo = GetInitializationInfo(p_ObjectTypePath, item, p_RoomTemplateObject, p_PreferredGridPosition, p_TileGrid);
            return new SpawnPoint(initializationInfo);
        }
    }

}
