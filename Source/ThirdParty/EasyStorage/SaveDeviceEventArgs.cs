using Microsoft.Xna.Framework;
using System;

namespace EasyStorage
{
    /// <summary>
    /// Event arguments for the SaveDevice class.
    /// </summary>
    public sealed class SaveDeviceEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the response to the event. The default response is to prompt.
        /// </summary>
        public SaveDeviceEventResponse Response { get; set; }

        /// <summary>
        /// Gets or sets the player index of the controller for which the message
        /// boxes should appear. This does not change the actual selection of the
        /// device but is merely used for the message box displays. Set to null
        /// to allow any player to handle the message box.
        /// </summary>
        public PlayerIndex? PlayerToPrompt { get; set; }
    }
}