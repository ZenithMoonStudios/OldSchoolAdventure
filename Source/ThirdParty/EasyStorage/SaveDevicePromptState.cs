namespace EasyStorage
{
    /// <summary>
    /// The various states of the SaveDevice.
    /// </summary>
    internal enum SaveDevicePromptState
    {
        /// <summary>
        /// The SaveDevice is doing nothing.
        /// </summary>
        None,

        /// <summary>
        /// The SaveDevice needs to show the StorageDevice selector.
        /// </summary>
        ShowSelector,

        /// <summary>
        /// The SaveDevice needs to prompt the user because a 
        /// StorageDevice selector was canceled.
        /// </summary>
        PromptForCanceled,

        /// <summary>
        /// The SaveDevice needs to force the user to choose a
        /// StorageDevice because a StorageDevice selector was canceled.
        /// </summary>
        ForceCanceledReselection,

        /// <summary>
        /// The SaveDevice needs to prompt the user because a 
        /// StorageDevice was disconnected.
        /// </summary>
        PromptForDisconnected,

        /// <summary>
        /// The SaveDevice needs to force the user to choose a
        /// StorageDevice because a StorageDevice was disconnected.
        /// </summary>
        ForceDisconnectedReselection
    }
}