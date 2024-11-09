namespace OSA
{
    public class GameState
    {
        public int ActiveFrames { get; set; }
        public int Level { get; set; } = -1;

        public DoorTarget LastDoor { get; set; }
        public CollectibleStore Collectibles { get; set; }
    }
}
