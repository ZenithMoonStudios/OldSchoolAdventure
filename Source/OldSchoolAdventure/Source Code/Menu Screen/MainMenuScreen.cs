using Destiny;
using Microsoft.Xna.Framework;

namespace OSA
{
    public class MainMenuScreen : MenuScreen
    {
        public MainMenuScreen() : base("Main Menu")
        {
            // New game
            MenuEntry newGameEntry = new MenuEntry("New Game", this);
            newGameEntry.Selected += NewGameSelected;
            this.Entries.Add(newGameEntry);

            // Load game
            MenuEntry loadGameEntry = new MenuEntry("Load Game", false, true, false, false, true, true, this, null);
            loadGameEntry.Selected += LoadGameSelected;
            this.Entries.Add(loadGameEntry);

            // View controls
            MenuEntry controlsEntry = new MenuEntry("View Controls", this);
            controlsEntry.Selected += ControlsSelected;
            this.Entries.Add(controlsEntry);

            // Credits
            MenuEntry creditsEntry = new MenuEntry("Credits", this);
            creditsEntry.Selected += CreditsSelected;
            this.Entries.Add(creditsEntry);

#if XBOX || MOBILE
			// Purchase
			MenuEntry purchaseEntry = new MenuEntry("Purchase", true, false, true, true, false, true, this, null);
			purchaseEntry.Selected += PurchaseGameSelected;
			this.Entries.Add(purchaseEntry);
#endif

#if XBOX
            // Other games
			MenuEntry otherGamesEntry = new MenuEntry("Other Games", this);
			otherGamesEntry.Selected += OtherGamesSelected;
			this.Entries.Add(otherGamesEntry);
#endif

            // Exit
#if XBOX
            string exitPrompt = "Return to Dashboard";
#else
            string exitPrompt = "Exit";
#endif
            MenuEntry exitEntry = new MenuEntry(exitPrompt, this);
            exitEntry.Selected += ExitSelected;
            this.Entries.Add(exitEntry);

            this.CanCancel = false;

            this.EnabledGestures = Microsoft.Xna.Framework.Input.Touch.GestureType.Tap;

        }

        void NewGameSelected(object p_Tag) { this.ScreenManager.AddScreen(new GameModeMenuScreen()); }
        void LoadGameSelected(object p_Tag) { PlatformerGame.Instance.LoadGame(); }
        void ControlsSelected(object p_Tag) { this.ScreenManager.AddScreen(new ControlsScreen()); }
        void CreditsSelected(object p_Tag) { this.ScreenManager.AddScreen(new CreditsScreen(false)); }
        void OtherGamesSelected(object p_Tag) { this.ScreenManager.AddScreen(new OtherGamesScreen()); }

        void PurchaseGameSelected(object p_Tag)
        {
            PlatformerGame.Instance.PurchasingComponent.RequestPurchase(this.OnPurchaseHandler, null);
        }
        void OnPurchaseHandler(bool p_IsPurchased, object p_Tag) { }

        void ExitSelected(object p_Tag)
        {
            MessageBoxScreen.Show(
                this.ScreenManager,
                "Exit Game?", "Are you sure you want to exit Old School Adventure?",
                "Yes", "No",
                this.ConfirmExitMessageBoxHandler
                );
        }

        void ConfirmExitMessageBoxHandler(bool p_IsAccept)
        {
            if (p_IsAccept)
            {
                if (GuideHelper.CanPurchase((PlayerIndex)this.InputState.ActivePlayerIndex))
                {
                    PlatformerGame.Instance.PurchasingComponent.RequestPurchase(this.OnExitPurchaseHandler, null);
                }
                else
                {
                    this.Game.Exit();
                }
            }
        }

        void OnExitPurchaseHandler(bool p_IsPurchased, object p_Tag)
        {
            this.Game.Exit();
        }

        /// <summary>
        /// When the user cancels the main menu, we exit the game.
        /// </summary>
        protected override void OnCancel()
        {
            MessageBoxScreen.Show(
                this.ScreenManager,
                "Exit Game?", "Are you sure you want to exit Old School Adventure?",
                "Yes", "No",
                this.ConfirmExitMessageBoxHandler
                );
        }
    }
}
