using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading;

namespace EasyStorage
{
    /// <summary>
    /// An implementation of ISaveDevice utilizing IsolatedStorage for Windows Phone.
    /// Given that this is an ISaveDevice, all of our methods still have a container
    /// name for compatibility, but that parameter is simply ignored by the
    /// implementation since there is no concept of a container within IsolatedStorage.
    /// </summary>
    public class IsolatedStorageSaveDevice : IAsyncSaveDevice
    {
        // a queue used for recycling our state objects to avoid garbage or boxing when using ThreadPool
        private Queue<FileOperationState> pendingStates = new Queue<FileOperationState>(100);

        private readonly object pendingOperationCountLock = new object();
        private int pendingOperations;

        /// <summary>
        /// Gets whether or not the device is in a state to receive file operation method calls.
        /// </summary>
        public bool IsReady { get { return true; } }

        /// <summary>
        /// Gets whether or not the device is busy performing a file operation.
        /// </summary>
        /// <remarks>
        /// Games can query this property to determine when to show an indication that game is saving
        /// such as a spinner or other icon.
        /// </remarks>
        public bool IsBusy
        {
            get
            {
                lock (pendingOperationCountLock)
                {
                    return pendingOperations > 0;
                }
            }
        }

        /// <summary>
        /// Raised when a SaveAsync operation has completed.
        /// </summary>
        public event SaveCompletedEventHandler SaveCompleted;

        /// <summary>
        /// Raised when a LoadAsync operation has completed.
        /// </summary>
        public event LoadCompletedEventHandler LoadCompleted;

        /// <summary>
        /// Raised when a DeleteAsync operation has completed.
        /// </summary>
        public event DeleteCompletedEventHandler DeleteCompleted;

        /// <summary>
        /// Raised when a FileExistsAsync operation has completed.
        /// </summary>
        public event FileExistsCompletedEventHandler FileExistsCompleted;

        /// <summary>
        /// Raised when a GetFilesAsync operation has completed.
        /// </summary>
        public event GetFilesCompletedEventHandler GetFilesCompleted;

        /// <summary>
        /// Helper method that gets isolated storage. On Windows we use GetUserStoreForDomain, but on the other
        /// platforms we use GetUserStoreForApplication.
        /// </summary>
        /// <returns>The opened IsolatedStorageFile.</returns>
        private IsolatedStorageFile GetIsolatedStorage()
        {
#if WINDOWS
			return IsolatedStorageFile.GetUserStoreForDomain();
#else
            return IsolatedStorageFile.GetUserStoreForApplication();
#endif
        }

        /// <summary>
        /// Saves a file.
        /// </summary>
        /// <param name="containerName">Used to match the ISaveDevice interface; ignored by the implementation.</param>
        /// <param name="fileName">The file to save.</param>
        /// <param name="saveAction">The save action to perform.</param>
        public void Save(string containerName, string fileName, FileAction saveAction)
        {
            using (IsolatedStorageFile storage = GetIsolatedStorage())
            {
                using (var stream = storage.CreateFile(fileName))
                {
                    saveAction(stream);
                }
            }
        }

        /// <summary>
        /// Loads a file.
        /// </summary>
        /// <param name="containerName">Used to match the ISaveDevice interface; ignored by the implementation.</param>
        /// <param name="fileName">The file to load.</param>
        /// <param name="loadAction">The load action to perform.</param>
        public void Load(string containerName, string fileName, FileAction loadAction)
        {
            using (IsolatedStorageFile storage = GetIsolatedStorage())
            {
                using (var stream = storage.OpenFile(fileName, FileMode.Open))
                {
                    loadAction(stream);
                }
            }
        }

        /// <summary>
        /// Deletes a file.
        /// </summary>
        /// <param name="containerName">Used to match the ISaveDevice interface; ignored by the implementation.</param>
        /// <param name="fileName">The file to delete.</param>
        public void Delete(string containerName, string fileName)
        {
            using (IsolatedStorageFile storage = GetIsolatedStorage())
            {
                if (storage.FileExists(fileName))
                {
                    storage.DeleteFile(fileName);
                }
            }
        }

        /// <summary>
        /// Determines if a given file exists.
        /// </summary>
        /// <param name="containerName">Used to match the ISaveDevice interface; ignored by the implementation.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>True if the file exists, false otherwise.</returns>
        public bool FileExists(string containerName, string fileName)
        {
            using (IsolatedStorageFile storage = GetIsolatedStorage())
            {
                return storage.FileExists(fileName);
            }
        }

        /// <summary>
        /// Gets an array of all files available in a container.
        /// </summary>
        /// <param name="containerName">Used to match the ISaveDevice interface; ignored by the implementation.</param>
        /// <returns>An array of file names of the files in the container.</returns>
        public string[] GetFiles(string containerName)
        {
            using (IsolatedStorageFile storage = GetIsolatedStorage())
            {
                return storage.GetFileNames();
            }
        }

        /// <summary>
        /// Gets an array of all files available in a container.
        /// </summary>
        /// <param name="containerName">Used to match the ISaveDevice interface; ignored by the implementation.</param>
        /// <param name="pattern">A search pattern to use to find files.</param>
        /// <returns>An array of file names of the files in the container.</returns>
        public string[] GetFiles(string containerName, string pattern)
        {
            using (IsolatedStorageFile storage = GetIsolatedStorage())
            {
                return storage.GetFileNames(pattern);
            }
        }

        /// <summary>
        /// Saves a file asynchronously.
        /// </summary>
        /// <param name="containerName">The name of the container in which to save the file.</param>
        /// <param name="fileName">The file to save.</param>
        /// <param name="saveAction">The save action to perform.</param>
        public void SaveAsync(string containerName, string fileName, FileAction saveAction)
        {
            SaveAsync(containerName, fileName, saveAction, null);
        }

        /// <summary>
        /// Saves a file asynchronously.
        /// </summary>
        /// <param name="containerName">The name of the container in which to save the file.</param>
        /// <param name="fileName">The file to save.</param>
        /// <param name="saveAction">The save action to perform.</param>
        /// <param name="userState">A state object used to identify the async operation.</param>
        public void SaveAsync(string containerName, string fileName, FileAction saveAction, object userState)
        {
            // increment our pending operations count
            PendingOperationsIncrement();

            // get a FileOperationState and fill it in
            FileOperationState state = GetFileOperationState();
            state.Container = containerName;
            state.File = fileName;
            state.Action = saveAction;
            state.UserState = userState;

            // queue up the work item
            ThreadPool.QueueUserWorkItem(DoSaveAsync, state);
        }

        /// <summary>
        /// Loads a file asynchronously.
        /// </summary>
        /// <param name="containerName">The name of the container from which to load the file.</param>
        /// <param name="fileName">The file to load.</param>
        /// <param name="loadAction">The load action to perform.</param>
        public void LoadAsync(string containerName, string fileName, FileAction loadAction)
        {
            LoadAsync(containerName, fileName, loadAction, null);
        }

        /// <summary>
        /// Loads a file asynchronously.
        /// </summary>
        /// <param name="containerName">The name of the container from which to load the file.</param>
        /// <param name="fileName">The file to load.</param>
        /// <param name="loadAction">The load action to perform.</param>
        /// <param name="userState">A state object used to identify the async operation.</param>
        public void LoadAsync(string containerName, string fileName, FileAction loadAction, object userState)
        {
            // increment our pending operations count
            PendingOperationsIncrement();

            // get a FileOperationState and fill it in
            FileOperationState state = GetFileOperationState();
            state.Container = containerName;
            state.File = fileName;
            state.Action = loadAction;
            state.UserState = userState;

            // queue up the work item
            ThreadPool.QueueUserWorkItem(DoLoadAsync, state);
        }

        /// <summary>
        /// Deletes a file asynchronously.
        /// </summary>
        /// <param name="containerName">The name of the container from which to delete the file.</param>
        /// <param name="fileName">The file to delete.</param>
        public void DeleteAsync(string containerName, string fileName)
        {
            DeleteAsync(containerName, fileName, null);
        }

        /// <summary>
        /// Deletes a file asynchronously.
        /// </summary>
        /// <param name="containerName">The name of the container from which to delete the file.</param>
        /// <param name="fileName">The file to delete.</param>
        /// <param name="userState">A state object used to identify the async operation.</param>
        public void DeleteAsync(string containerName, string fileName, object userState)
        {
            // increment our pending operations count
            PendingOperationsIncrement();

            // get a FileOperationState and fill it in
            FileOperationState state = GetFileOperationState();
            state.Container = containerName;
            state.File = fileName;
            state.UserState = userState;

            // queue up the work item
            ThreadPool.QueueUserWorkItem(DoDeleteAsync, state);
        }

        /// <summary>
        /// Determines if a given file exists asynchronously.
        /// </summary>
        /// <param name="containerName">The name of the container in which to check for the file.</param>
        /// <param name="fileName">The name of the file.</param>
        public void FileExistsAsync(string containerName, string fileName)
        {
            FileExistsAsync(containerName, fileName, null);
        }

        /// <summary>
        /// Determines if a given file exists asynchronously.
        /// </summary>
        /// <param name="containerName">The name of the container in which to check for the file.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="userState">A state object used to identify the async operation.</param>
        public void FileExistsAsync(string containerName, string fileName, object userState)
        {
            // increment our pending operations count
            PendingOperationsIncrement();

            // get a FileOperationState and fill it in
            FileOperationState state = GetFileOperationState();
            state.Container = containerName;
            state.File = fileName;
            state.UserState = userState;

            // queue up the work item
            ThreadPool.QueueUserWorkItem(DoFileExistsAsync, state);
        }

        /// <summary>
        /// Gets an array of all files available in a container asynchronously.
        /// </summary>
        /// <param name="containerName">The name of the container in which to search for files.</param>
        public void GetFilesAsync(string containerName)
        {
            GetFilesAsync(containerName, null);
        }

        /// <summary>
        /// Gets an array of all files available in a container asynchronously.
        /// </summary>
        /// <param name="containerName">The name of the container in which to search for files.</param>
        /// <param name="userState">A state object used to identify the async operation.</param>
        public void GetFilesAsync(string containerName, object userState)
        {
            GetFilesAsync(containerName, "*", userState);
        }

        /// <summary>
        /// Gets an array of all files available in a container that match the given pattern asynchronously.
        /// </summary>
        /// <param name="containerName">The name of the container in which to search for files.</param>
        /// <param name="pattern">A search pattern to use to find files.</param>
        public void GetFilesAsync(string containerName, string pattern)
        {
            GetFilesAsync(containerName, pattern, null);
        }

        /// <summary>
        /// Gets an array of all files available in a container that match the given pattern asynchronously.
        /// </summary>
        /// <param name="containerName">The name of the container in which to search for files.</param>
        /// <param name="pattern">A search pattern to use to find files.</param>
        /// <param name="userState">A state object used to identify the async operation.</param>
        public void GetFilesAsync(string containerName, string pattern, object userState)
        {
            // increment our pending operations count
            PendingOperationsIncrement();

            // get a FileOperationState and fill it in
            FileOperationState state = GetFileOperationState();
            state.Container = containerName;
            state.Pattern = pattern;
            state.UserState = userState;

            // queue up the work item
            ThreadPool.QueueUserWorkItem(DoGetFilesAsync, state);
        }

        /// <summary>
        /// Helper that performs our asynchronous saving.
        /// </summary>
        private void DoSaveAsync(object asyncState)
        {
            FileOperationState state = asyncState as FileOperationState;
            Exception error = null;

            // perform the save operation
            try
            {
                Save(state.Container, state.File, state.Action);
            }
            catch (Exception e)
            {
                error = e;
            }

            // construct our event arguments
            FileActionCompletedEventArgs args = new FileActionCompletedEventArgs(error, state.UserState);

            // fire our completion event
            SaveCompleted(this, args);

            // recycle our state object
            ReturnFileOperationState(state);

            // decrement our pending operation count
            PendingOperationsDecrement();
        }

        /// <summary>
        /// Helper that performs our asynchronous loading.
        /// </summary>
        private void DoLoadAsync(object asyncState)
        {
            FileOperationState state = asyncState as FileOperationState;
            Exception error = null;

            // perform the load operation
            try
            {
                Load(state.Container, state.File, state.Action);
            }
            catch (Exception e)
            {
                error = e;
            }

            // construct our event arguments
            FileActionCompletedEventArgs args = new FileActionCompletedEventArgs(error, state.UserState);

            // fire our completion event
            LoadCompleted(this, args);

            // recycle our state object
            ReturnFileOperationState(state);

            // decrement our pending operation count
            PendingOperationsDecrement();
        }

        /// <summary>
        /// Helper that performs our asynchronous deleting.
        /// </summary>
        private void DoDeleteAsync(object asyncState)
        {
            FileOperationState state = asyncState as FileOperationState;
            Exception error = null;

            // perform the delete operation
            try
            {
                Delete(state.Container, state.File);
            }
            catch (Exception e)
            {
                error = e;
            }

            // construct our event arguments
            FileActionCompletedEventArgs args = new FileActionCompletedEventArgs(error, state.UserState);

            // fire our completion event
            DeleteCompleted(this, args);

            // recycle our state object
            ReturnFileOperationState(state);

            // decrement our pending operation count
            PendingOperationsDecrement();
        }

        /// <summary>
        /// Helper that performs our asynchronous FileExists.
        /// </summary>
        private void DoFileExistsAsync(object asyncState)
        {
            FileOperationState state = asyncState as FileOperationState;
            Exception error = null;
            bool result = false;

            // perform the FileExists operation
            try
            {
                result = FileExists(state.Container, state.File);
            }
            catch (Exception e)
            {
                error = e;
            }

            // construct our event arguments
            FileExistsCompletedEventArgs args = new FileExistsCompletedEventArgs(error, result, state.UserState);

            // fire our completion event
            FileExistsCompleted(this, args);

            // recycle our state object
            ReturnFileOperationState(state);

            // decrement our pending operation count
            PendingOperationsDecrement();
        }

        /// <summary>
        /// Helper that performs our asynchronous GetFiles.
        /// </summary>
        private void DoGetFilesAsync(object asyncState)
        {
            FileOperationState state = asyncState as FileOperationState;
            Exception error = null;
            string[] result = null;

            // perform the GetFiles operation
            try
            {
                result = GetFiles(state.Container, state.Pattern);
            }
            catch (Exception e)
            {
                error = e;
            }

            // construct our event arguments
            GetFilesCompletedEventArgs args = new GetFilesCompletedEventArgs(error, result, state.UserState);

            // fire our completion event
            GetFilesCompleted(this, args);

            // recycle our state object
            ReturnFileOperationState(state);

            // decrement our pending operation count
            PendingOperationsDecrement();
        }

        /// <summary>
        /// Helper to increment the pending operation count.
        /// </summary>
        private void PendingOperationsIncrement()
        {
            lock (pendingOperationCountLock)
                pendingOperations++;
        }

        /// <summary>
        /// Helper to decrement the pending operation count.
        /// </summary>
        private void PendingOperationsDecrement()
        {
            lock (pendingOperationCountLock)
                pendingOperations--;
        }

        /// <summary>
        /// Helper for getting a FileOperationState object.
        /// </summary>
        private FileOperationState GetFileOperationState()
        {
            lock (pendingStates)
            {
                // recycle any states if we have some available
                if (pendingStates.Count > 0)
                {
                    FileOperationState state = pendingStates.Dequeue();
                    state.Reset();
                    return state;
                }

                return new FileOperationState();
            }
        }

        /// <summary>
        /// Helper for returning a FileOperationState to be recycled.
        /// </summary>
        private void ReturnFileOperationState(FileOperationState state)
        {
            lock (pendingStates)
            {
                pendingStates.Enqueue(state);
            }
        }

        /// <summary>
        /// State object used for our operations.
        /// </summary>
        class FileOperationState
        {
            public string Container;
            public string File;
            public string Pattern;
            public FileAction Action;
            public object UserState;

            public void Reset()
            {
                Container = null;
                File = null;
                Pattern = null;
                Action = null;
                UserState = null;
            }
        }
    }
}
