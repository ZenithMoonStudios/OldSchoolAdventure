using Destiny;
using Microsoft.Xna.Framework.Content;
using System.IO;
using System.Xml.Serialization;

namespace OSA
{
    /// <summary>
    /// Level factory class.
    /// </summary>
    public static class LevelFactory
    {
        /// <summary>
        /// Load level.
        /// </summary>
        public static Level Load(string p_Name, string p_LevelPath, ContentManager p_ContentManager)
        {
            Level level = null;
            OSATypes.LevelIntroduction LevelIntroduction = new OSATypes.LevelIntroduction();
            OSATypes.LevelRooms levelRooms = new OSATypes.LevelRooms();
            OSATypes.LevelStartDoor LevelStartDoor = new OSATypes.LevelStartDoor();
            OSATypes.LevelEndDoor LevelEndDoor = new OSATypes.LevelEndDoor();

            XmlSerializer serializer = new XmlSerializer(typeof(OSATypes.Level));
#if(MOBILE)
			Stream fs = TitleContainer.OpenStream(Path.Combine(p_ContentManager.RootDirectory, p_LevelPath + ".xml"));
#else
            FileStream fs = new FileStream(Path.Combine(p_ContentManager.RootDirectory, p_LevelPath + ".xml"), FileMode.Open);
#endif
            OSATypes.Level levelLoad;
            levelLoad = (OSATypes.Level)serializer.Deserialize(fs);
            foreach (object item in levelLoad.Items)
            {
                switch (item.GetType().ToString())
                {
                    case "OSATypes.LevelIntroduction":
                        LevelIntroduction = (OSATypes.LevelIntroduction)item;
                        break;
                    case "OSATypes.LevelRooms":
                        levelRooms = (OSATypes.LevelRooms)item;
                        break;
                    case "OSATypes.LevelStartDoor":
                        LevelStartDoor = (OSATypes.LevelStartDoor)item;
                        break;
                    case "OSATypes.LevelEndDoor":
                        LevelEndDoor = (OSATypes.LevelEndDoor)item;
                        break;
                }
            }


            LevelIntroPageList introPages = new LevelIntroPageList();
            if (LevelIntroduction.Page != null && LevelIntroduction.Page.Count > 0)
                foreach (OSATypes.LevelIntroductionPage page in LevelIntroduction.Page)
                {
                    StringList introPageLines = new StringList();
                    foreach (OSATypes.LevelIntroductionPageLine Line in page.LineField)
                    {
                        introPageLines.Add(Line.Value);
                    }
                }

            StringList rooms = new StringList();
            foreach (OSATypes.LevelRoomsRoom room in levelRooms.Room)
            {
                if (!string.IsNullOrEmpty(room.Path))
                {
                    rooms.Add(room.Path);
                }
            }

            DoorTarget startDoor = new DoorTarget() { ObjectName = LevelStartDoor.ObjectName, RoomPath = LevelStartDoor.Room };
            DoorTarget endDoor = new DoorTarget() { ObjectName = LevelEndDoor.ObjectName, RoomPath = LevelEndDoor.Room };

            level = new Level(introPages, rooms, startDoor, endDoor);
            level.Name = p_Name;

            return level;
        }

    }
}
