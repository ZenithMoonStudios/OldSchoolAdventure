
namespace OSA
{
    public class GameModeMenuScreen : MenuScreen
    {
        public GameModeMenuScreen() : base("Choose Mode")
        {
            foreach (PlatformerGameSettings.GameMode mode in PlatformerGame.Instance.GameSettings.Modes)
            {
                MenuEntry entry = new MenuEntry(mode.Name, this, mode);
                entry.Selected += ModeSelected;
                this.Entries.Add(entry);
            }
            this.EnabledGestures = Microsoft.Xna.Framework.Input.Touch.GestureType.Tap;
        }

        void ModeSelected(object p_Tag) { PlatformerGame.Instance.NewGame((PlatformerGameSettings.GameMode)p_Tag); }
    }
}
