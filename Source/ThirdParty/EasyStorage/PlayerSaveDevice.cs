using Microsoft.Xna.Framework;
using System;

namespace EasyStorage
{
    /// <summary>
    /// A SaveDevice used for saving player-specific data.
    /// </summary>
    public sealed class PlayerSaveDevice : SaveDevice
    {
        // the format used for our exception message
        private const string playerException = "Player {0} must be signed in to get a player specific storage device.";

        /// <summary>
        /// Gets the PlayerIndex of the player for which the data will be saved.
        /// </summary>
        public PlayerIndex Player { get; private set; }

        /// <summary>
        /// Creates a new PlayerSaveDevice for a given player.
        /// </summary>
        /// <param name="player">The player for which the data will be saved.</param>
        public PlayerSaveDevice(PlayerIndex player)
        {
            Player = player;
        }

        /// <summary>
        /// Derived classes should implement this method to call the Guide.BeginShowStorageDeviceSelector
        /// method with the desired parameters, using the given callback.
        /// </summary>
        /// <param name="callback">The callback to pass to Guide.BeginShowStorageDeviceSelector.</param>
        protected override void GetStorageDevice(AsyncCallback callback)
        {
#if XBOX
			// gamers are required to be signed in to open a container and 
			// save files. an exception is raised by OpenContainer if a user 
			// is not signed in, but we want to be more proactive about this 
			// and throw an exception before even prompting the user in case
			// a game doesn't happen to save a file while testing. this 
			// should help games in peer review hit this exception more 
			// easily in case the tester does not trigger the game to save 
			// data on a profile that isn't signed in.

			if (SignedInGamer.SignedInGamers[Player] == null)
				throw new InvalidOperationException(string.Format(playerException, Player));
#endif

            StorageDevice.BeginShowSelector(Player, callback, null);
        }

        /// <summary>
        /// Prepares the SaveDeviceEventArgs to be used for an event.
        /// </summary>
        /// <param name="args">The event arguments to be configured.</param>
        protected override void PrepareEventArgs(SaveDeviceEventArgs args)
        {
            // the base implementation sets some aspects of the arguments,
            // so we let it do that first
            base.PrepareEventArgs(args);

            // we then default the player to prompt to be the player that
            // owns this storage device. we assume the game will leave this
            // untouched so that the correct player is prompted, but we also
            // allow the game to change it if there's a reason to.
            args.PlayerToPrompt = Player;
        }
    }
}