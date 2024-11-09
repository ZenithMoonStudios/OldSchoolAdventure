namespace EasyStorage
{
    /// <summary>
    /// Responses for a user canceling the StorageDevice selector
    /// or disconnecting the StorageDevice.
    /// </summary>
    public enum SaveDeviceEventResponse
    {
        /// <summary>
        /// Take no action.
        /// </summary>
        Nothing,

        /// <summary>
        /// Displays a message box to choose whether to select a new 
        /// device and shows the selector if appropriate.
        /// </summary>
        Prompt,

        /// <summary>
        /// Displays a message that the user must choose a new device
        /// and shows the device selector.
        /// </summary>
        Force,
    }
}