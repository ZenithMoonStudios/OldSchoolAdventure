using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace OSA
{
    /// <summary>
    /// Music point class.
    /// </summary>
    public class MusicPoint : RoomObject
    {
        public string Music { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public MusicPoint(string p_Music, RoomObjectInitialization p_Init) : base(p_Init)
        {
            this.Music = p_Music;
        }

        /// <summary>
        /// Handle collision.
        /// </summary>
        /// <param name="p_Object">Object that is being collided with.</param>
        /// <param name="p_IsCollision">Whether there is a collision.</param>
        protected override void HandleCollision(SceneObject p_Object, bool p_IsCollision)
        {
            if (this.IsAlive && p_Object is MainCharacter)
            {
                MainCharacter mainCharacter = p_Object as MainCharacter;
                if (p_IsCollision && mainCharacter.IsAlive)
                {
                    this.AudioManager.PlayMusic(this.Music);
                }
            }

            // Do default processing
            base.HandleCollision(p_Object, p_IsCollision);
        }
    }

    /// <summary>
    /// Music point list class.
    /// </summary>
    public class MusicPointList : List<MusicPoint> { }

    /// <summary>
    /// String to music point dictionary class.
    /// </summary>
    public class StringToMusicPointDictionary : Dictionary<string, MusicPoint> { }

    /// <summary>
    /// Xml reader.
    /// </summary>
    public class XmlMusicPointReader : XmlRoomObjectReader
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public XmlMusicPointReader()
        {
        }

        /// <summary>
        /// Create an instance.
        /// </summary>
        public override RoomObject CreateInstance(string p_ObjectTypePath, OSATypes.RoomObjectItem item, OSATypes.RoomObjectTemplate p_RoomTemplateObject, Vector2 p_PreferredGridPosition, TileGrid p_TileGrid)
        {
            string music = p_RoomTemplateObject.Music != null ? p_RoomTemplateObject.Music : string.Empty;

            RoomObjectInitialization initializationInfo = GetInitializationInfo(p_ObjectTypePath, item, p_RoomTemplateObject, p_PreferredGridPosition, p_TileGrid);
            return new MusicPoint(music, initializationInfo);
        }
    }

}
