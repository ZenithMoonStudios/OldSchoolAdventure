using System;

namespace EasyStorage
{
    /// <summary>
    /// Defines the interface for an object that can perform the ISaveDevice operations in
    /// an asynchronous fashion.
    /// </summary>
    /// <remarks>
    /// This class was based on the event-based asynchronous pattern, which is the new
    /// recommended method for asynchronous APIs in .NET. For more information, see this page:
    /// http://msdn.microsoft.com/en-us/library/hkasytyf.aspx
    /// 
    /// Our pattern deviates from the standard in that our events don't use the standard
    /// EventHandler or AsyncCompletedEventArgs as that would cause us to generate garbage.
    /// Instead we have substituted those events with a delegate based event that uses structs
    /// for arguments, allowing us to avoid garbage generation.
    /// 
    /// Additionally, we choose, in the built in implementations, to not throw an exception if 
    /// a single-invocation method is called while an operation is pending. Instead we simply 
    /// queue it up without issue. This is done as a convenience to the game developer, even if 
    /// it does deviate from the pattern.
    /// 
    /// We also choose not to support cancellation due to the complexities of the implementation
    /// and because there is little benefit from the ability to cancel a storage operation which
    /// should be a relatively quick process in and of itself.
    /// </remarks>
    public interface IAsyncSaveDevice : ISaveDevice
    {
        /// <summary>
        /// Gets whether or not the device is busy performing a file operation.
        /// </summary>
        /// <remarks>
        /// Games can query this property to determine when to show an indication that game is saving
        /// such as a spinner or other icon.
        /// </remarks>
        bool IsBusy { get; }

        /// <summary>
        /// Raised when a SaveAsync operation has completed.
        /// </summary>
        event SaveCompletedEventHandler SaveCompleted;

        /// <summary>
        /// Raised when a LoadAsync operation has completed.
        /// </summary>
        event LoadCompletedEventHandler LoadCompleted;

        /// <summary>
        /// Raised when a DeleteAsync operation has completed.
        /// </summary>
        event DeleteCompletedEventHandler DeleteCompleted;

        /// <summary>
        /// Raised when a FileExistsAsync operation has completed.
        /// </summary>
        event FileExistsCompletedEventHandler FileExistsCompleted;

        /// <summary>
        /// Raised when a GetFilesAsync operation has completed.
        /// </summary>
        event GetFilesCompletedEventHandler GetFilesCompleted;

        /// <summary>
        /// Saves a file asynchronously.
        /// </summary>
        /// <param name="containerName">The name of the container in which to save the file.</param>
        /// <param name="fileName">The file to save.</param>
        /// <param name="saveAction">The save action to perform.</param>
        void SaveAsync(string containerName, string fileName, FileAction saveAction);

        /// <summary>
        /// Saves a file asynchronously.
        /// </summary>
        /// <param name="containerName">The name of the container in which to save the file.</param>
        /// <param name="fileName">The file to save.</param>
        /// <param name="saveAction">The save action to perform.</param>
        /// <param name="userState">A state object used to identify the async operation.</param>
        void SaveAsync(string containerName, string fileName, FileAction saveAction, object userState);

        /// <summary>
        /// Loads a file asynchronously.
        /// </summary>
        /// <param name="containerName">The name of the container from which to load the file.</param>
        /// <param name="fileName">The file to load.</param>
        /// <param name="loadAction">The load action to perform.</param>
        void LoadAsync(string containerName, string fileName, FileAction loadAction);

        /// <summary>
        /// Loads a file asynchronously.
        /// </summary>
        /// <param name="containerName">The name of the container from which to load the file.</param>
        /// <param name="fileName">The file to load.</param>
        /// <param name="loadAction">The load action to perform.</param>
        /// <param name="userState">A state object used to identify the async operation.</param>
        void LoadAsync(string containerName, string fileName, FileAction loadAction, object userState);

        /// <summary>
        /// Deletes a file asynchronously.
        /// </summary>
        /// <param name="containerName">The name of the container from which to delete the file.</param>
        /// <param name="fileName">The file to delete.</param>
        void DeleteAsync(string containerName, string fileName);

        /// <summary>
        /// Deletes a file asynchronously.
        /// </summary>
        /// <param name="containerName">The name of the container from which to delete the file.</param>
        /// <param name="fileName">The file to delete.</param>
        /// <param name="userState">A state object used to identify the async operation.</param>
        void DeleteAsync(string containerName, string fileName, object userState);

        /// <summary>
        /// Determines if a given file exists asynchronously.
        /// </summary>
        /// <param name="containerName">The name of the container in which to check for the file.</param>
        /// <param name="fileName">The name of the file.</param>
        void FileExistsAsync(string containerName, string fileName);

        /// <summary>
        /// Determines if a given file exists asynchronously.
        /// </summary>
        /// <param name="containerName">The name of the container in which to check for the file.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="userState">A state object used to identify the async operation.</param>
        void FileExistsAsync(string containerName, string fileName, object userState);

        /// <summary>
        /// Gets an array of all files available in a container asynchronously.
        /// </summary>
        /// <param name="containerName">The name of the container in which to search for files.</param>
        void GetFilesAsync(string containerName);

        /// <summary>
        /// Gets an array of all files available in a container asynchronously.
        /// </summary>
        /// <param name="containerName">The name of the container in which to search for files.</param>
        /// <param name="userState">A state object used to identify the async operation.</param>
        void GetFilesAsync(string containerName, object userState);

        /// <summary>
        /// Gets an array of all files available in a container that match the given pattern asynchronously.
        /// </summary>
        /// <param name="containerName">The name of the container in which to search for files.</param>
        /// <param name="pattern">A search pattern to use to find files.</param>
        void GetFilesAsync(string containerName, string pattern);

        /// <summary>
        /// Gets an array of all files available in a container that match the given pattern asynchronously.
        /// </summary>
        /// <param name="containerName">The name of the container in which to search for files.</param>
        /// <param name="pattern">A search pattern to use to find files.</param>
        /// <param name="userState">A state object used to identify the async operation.</param>
        void GetFilesAsync(string containerName, string pattern, object userState);
    }

    /// <summary>
    /// Used for the arguments for SaveAsync, LoadAsync, and DeleteAsync.
    /// </summary>
    public struct FileActionCompletedEventArgs
    {
        /// <summary>
        /// The exception, if any, that occurred during the operation.
        /// </summary>
        public Exception Error { get; private set; }

        /// <summary>
        /// The user state passed into the async method.
        /// </summary>
        public object UserState { get; private set; }

        public FileActionCompletedEventArgs(Exception error, object userState)
            : this()
        {
            Error = error;
            UserState = userState;
        }
    }

    /// <summary>
    /// Used for arguments for FileExistsAsync.
    /// </summary>
    public struct FileExistsCompletedEventArgs
    {
        /// <summary>
        /// The exception, if any, that occurred during the operation.
        /// </summary>
        public Exception Error { get; private set; }

        /// <summary>
        /// The user state passed into the async method.
        /// </summary>
        public object UserState { get; private set; }

        /// <summary>
        /// Whether or not the file exists.
        /// </summary>
        public bool Result { get; private set; }

        public FileExistsCompletedEventArgs(Exception error, bool result, object userState)
            : this()
        {
            Error = error;
            Result = result;
            UserState = userState;
        }
    }

    /// <summary>
    /// Used for arguments for GetFilesAsync.
    /// </summary>
    public struct GetFilesCompletedEventArgs
    {
        /// <summary>
        /// The exception, if any, that occurred during the operation.
        /// </summary>
        public Exception Error { get; private set; }

        /// <summary>
        /// The user state passed into the async method.
        /// </summary>
        public object UserState { get; private set; }

        /// <summary>
        /// The list of files returned by the operation.
        /// </summary>
        public string[] Result { get; private set; }

        public GetFilesCompletedEventArgs(Exception error, string[] result, object userState)
            : this()
        {
            Error = error;
            Result = result;
            UserState = userState;
        }
    }

    /// <summary>
    /// Defines an event handler signature for handling the SaveCompleted event.
    /// </summary>
    /// <param name="sender">The IAsyncSaveDevice that raised the event.</param>
    /// <param name="args">The results of the operation.</param>
    public delegate void SaveCompletedEventHandler(object sender, FileActionCompletedEventArgs args);

    /// <summary>
    /// Defines an event handler signature for handling the LoadCompleted event.
    /// </summary>
    /// <param name="sender">The IAsyncSaveDevice that raised the event.</param>
    /// <param name="args">The results of the operation.</param>
    public delegate void LoadCompletedEventHandler(object sender, FileActionCompletedEventArgs args);

    /// <summary>
    /// Defines an event handler signature for handling the DeleteCompleted event.
    /// </summary>
    /// <param name="sender">The IAsyncSaveDevice that raised the event.</param>
    /// <param name="args">The results of the operation.</param>
    public delegate void DeleteCompletedEventHandler(object sender, FileActionCompletedEventArgs args);

    /// <summary>
    /// Defines an event handler signature for handling the FileExistsCompleted event.
    /// </summary>
    /// <param name="sender">The IAsyncSaveDevice that raised the event.</param>
    /// <param name="args">The results of the operation.</param>
    public delegate void FileExistsCompletedEventHandler(object sender, FileExistsCompletedEventArgs args);

    /// <summary>
    /// Defines an event handler signature for handling the GetFilesCompleted event.
    /// </summary>
    /// <param name="sender">The IAsyncSaveDevice that raised the event.</param>
    /// <param name="args">The results of the operation.</param>
    public delegate void GetFilesCompletedEventHandler(object sender, GetFilesCompletedEventArgs args);
}
