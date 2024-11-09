using System;

namespace EasyStorage
{
    /// <summary>
    /// Event arguments for the SaveDevice after a MessageBox prompt
    /// has been closed.
    /// </summary>
    public sealed class SaveDevicePromptEventArgs : EventArgs
    {
        /// <summary>
        /// Gets whether or not the user has chosen to select a new
        /// StorageDevice.
        /// </summary>
        public bool ShowDeviceSelector { get; internal set; }
    }
}