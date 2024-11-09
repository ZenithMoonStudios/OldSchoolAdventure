namespace OSA
{
    /// <summary>
    /// Game data class.
    /// </summary>
    public class PlatformerGameData
    {
        /// <summary>
        /// Player data.
        /// </summary>
        public MainCharacter Player { get; private set; }
        public CollectibleStore Store { get { return this.Player.Store; } }

        /// <summary>
        /// Timer data.
        /// </summary>
        public int ActiveFrames { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public PlatformerGameData() : this(0, new CollectibleStore()) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        public PlatformerGameData(int p_ActiveFrames, CollectibleStore p_Store)
        {
            this.ActiveFrames = p_ActiveFrames;

            // Load main character template
            string objectTypePath = "Character/Chris";
            RoomObjectTemplateManager templateManager = new RoomObjectTemplateManager("Content/Level Play Screen");
            RoomObjectTemplate template = templateManager.Get(objectTypePath);
            RoomObjectInitialization initInfo = XmlRoomObjectReader.GetInitializationInfo(objectTypePath, null, template.RoomTemplateObject);
            this.Player = new MainCharacter(p_Store, initInfo);
        }

        /// <summary>
        /// Submit level data.
        /// </summary>
        public void SubmitLevel(int p_ActiveFrames)
        {
            this.ActiveFrames += p_ActiveFrames;
        }
    }
}
