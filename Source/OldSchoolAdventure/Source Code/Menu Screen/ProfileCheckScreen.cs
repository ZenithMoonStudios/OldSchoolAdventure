using Destiny;
using Microsoft.Xna.Framework;
using System;

namespace OSA
{
    /// <summary>
    /// Profile check screen class.
    /// </summary>
    public class ProfileCheckScreen : Screen
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ProfileCheckScreen()
        {
            this.TransitionOnTime = TimeSpan.FromSeconds(0.5);
            this.IsPopup = true;
        }

#if !XBOX
        /// <summary>
        /// Handle input.
        /// Only called when screen has focus.
        /// </summary>
        /// <param name="p_InputState">Input state.</param>
        public override void HandleInput(InputState p_InputState)
        {
            base.HandleInput(p_InputState);

            UserProfile userProfile = new UserProfile((PlayerIndex)p_InputState.ActivePlayerIndex, "Player");
            ((PlatformerGame)this.Game).UserProfile = userProfile;

            // Initialize new save device
            userProfile.Initialize();

            // Navigate to main menu
            this.ScreenManager.AddScreen(new MainMenuScreen());
            this.ExitScreen();
        }
#else
		public enum States { CheckHasSignedInProfile, ConfirmLimitedProfile, HandleSignInResult, LoadMainMenu }
		private States m_State = States.CheckHasSignedInProfile;

		/// <summary>
		/// Handle input.
		/// Only called when screen has focus.
		/// </summary>
		/// <param name="p_InputState">Input state.</param>
		public override void HandleInput(InputState p_InputState)
		{
			base.HandleInput(p_InputState);

			if (m_State == States.CheckHasSignedInProfile)
			{
				// Check if player is signed in
				SignedInGamer signedInGamer = GetSignedInGamer(p_InputState);
				if (signedInGamer != null && !signedInGamer.IsGuest)
				{
					m_State = States.LoadMainMenu;
				}
				else
				{
					if (!Guide.IsVisible && this.State == ScreenState.Active)
					{
						Guide.ShowSignIn(1, false);
						m_State = States.HandleSignInResult;
					}
				}
			}
			else if (m_State == States.HandleSignInResult)
			{
				// Check if player is signed in
				SignedInGamer signedInGamer = GetSignedInGamer(p_InputState);
				if (signedInGamer != null && !signedInGamer.IsGuest)
				{
					m_State = States.LoadMainMenu;
				}
				else
				{
					if (!Guide.IsVisible)
					{
						if (signedInGamer != null)
						{
							// Guest profile
							MessageBoxScreen.Show(
								this.ScreenManager,
								"Confirm Guest Profile?",
								"When using a guest profile, you cannot load or save your progress. Are you sure you want to use a guest profile?",
								"Yes", "No",
								this.ConfirmLimitedProfileMessageBoxHandler
								);
						}
						else
						{
							// No profile
							MessageBoxScreen.Show(
								this.ScreenManager,
								"Confirm No Profile?",
								"When playing without a profile, you cannot load or save your progress. Are you sure you want to play without a profile?",
								"Yes", "No",
								this.ConfirmLimitedProfileMessageBoxHandler
								);
						}
					}
				}
			}
			else if (m_State == States.LoadMainMenu)
			{
				// Set profile status
				SignedInGamer signedInGamer = GetSignedInGamer(p_InputState);
				p_InputState.IsActivePlayerUsingSignedInProfile = (signedInGamer != null);
				p_InputState.IsActivePlayerUsingGuestProfile = (signedInGamer != null && signedInGamer.IsGuest);

				// Load user profile details
				if (signedInGamer != null && !signedInGamer.IsGuest)
				{
					UserProfile userProfile = ((PlatformerGame)this.Game).UserProfile;
					if (userProfile == null ||
						(userProfile.PlayerIndex != p_InputState.ActivePlayerIndex && userProfile.GamerTag != signedInGamer.Gamertag)
					)
					{
						// Release old save device
						if (userProfile != null)
						{
							userProfile.DeInitialize();
						}

						// Create new user profile
						userProfile = new UserProfile((PlayerIndex)p_InputState.ActivePlayerIndex, signedInGamer.Gamertag);
						((PlatformerGame)this.Game).UserProfile = userProfile;

						// Initialize new save device
						userProfile.Initialize();
					}
				}

				// Navigate to main menu
				this.ScreenManager.AddScreen(new MainMenuScreen());
				this.ExitScreen();
			}
		}

		/// <summary>
		/// Confirm limited profile message box callback.
		/// </summary>
		void ConfirmLimitedProfileMessageBoxHandler(bool p_IsAccept)
		{
			if (p_IsAccept)
			{
				m_State = States.LoadMainMenu;
			}
			else
			{
				m_State = States.CheckHasSignedInProfile;
			}
		}

		/// <summary>
		/// Whether user is signed in.
		/// </summary>
		static SignedInGamer GetSignedInGamer(InputState p_InputState)
		{
			return Gamer.SignedInGamers[(PlayerIndex)p_InputState.ActivePlayerIndex];
		}
#endif

        public override void Draw(GameTime p_GameTime) { }
    }
}
