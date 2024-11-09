using Destiny;

namespace OSA
{
    /// <summary>
    /// Level class.
    /// </summary>
    public class Level
    {
        public string Name { get; set; }

        public LevelIntroPageList IntroPages { get; private set; }

        public StringList Rooms { get; private set; }
        public DoorTarget StartDoor { get; private set; }
        public DoorTarget EndDoor { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Level(LevelIntroPageList p_IntroPages, StringList p_Rooms, DoorTarget p_StartDoor, DoorTarget p_EndDoor)
        {
            this.IntroPages = new LevelIntroPageList();
            foreach (LevelIntroPage page in p_IntroPages)
            {
                this.IntroPages.Add(page);
            }

            this.Rooms = new StringList();
            foreach (string room in p_Rooms)
            {
                this.Rooms.Add(room);
            }

            this.StartDoor = p_StartDoor;
            this.EndDoor = p_EndDoor;
        }
    }
}
