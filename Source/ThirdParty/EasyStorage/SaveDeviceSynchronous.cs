using System;
using System.IO;

namespace EasyStorage
{
    // implements the synchronous file operations for the SaveDevice.
    public abstract partial class SaveDevice
    {
        /// <summary>
        /// Helper method to open a StorageContainer.
        /// </summary>
        private StorageContainer OpenContainer(string containerName)
        {
            // open the container from the device. while this is normally an async process, we
            // block until completion which makes it a synchronous operation for our uses.
            IAsyncResult asyncResult = storageDevice.BeginOpenContainer(containerName, null, null);
            asyncResult.AsyncWaitHandle.WaitOne();
            return storageDevice.EndOpenContainer(asyncResult);
        }

        /// <summary>
        /// Helper that just checks that IsReady is true and throws if it's false.
        /// </summary>
        private void VerifyIsReady()
        {
            if (!IsReady)
                throw new InvalidOperationException(Strings.StorageDevice_is_not_valid);
        }

        /// <summary>
        /// Saves a file.
        /// </summary>
        /// <param name="containerName">The name of the container in which to save the file.</param>
        /// <param name="fileName">The file to save.</param>
        /// <param name="saveAction">The save action to perform.</param>
        public void Save(string containerName, string fileName, FileAction saveAction)
        {
            VerifyIsReady();

            // lock on the storage device so that only one storage operation can occur at a time
            lock (storageDevice)
            {
                // open a container
                using (StorageContainer currentContainer = OpenContainer(containerName))
                {
                    // attempt the save
                    using (var stream = currentContainer.CreateFile(fileName))
                    {
                        saveAction(stream);
                    }
                }
            }
        }

        /// <summary>
        /// Loads a file.
        /// </summary>
        /// <param name="containerName">The name of the container from which to load the file.</param>
        /// <param name="fileName">The file to load.</param>
        /// <param name="loadAction">The load action to perform.</param>
        public void Load(string containerName, string fileName, FileAction loadAction)
        {
            VerifyIsReady();

            // lock on the storage device so that only one storage operation can occur at a time
            lock (storageDevice)
            {
                // open a container
                using (StorageContainer currentContainer = OpenContainer(containerName))
                {
                    // attempt the load
                    using (var stream = currentContainer.OpenFile(fileName, FileMode.Open))
                    {
                        loadAction(stream);
                    }
                }
            }
        }

        /// <summary>
        /// Deletes a file.
        /// </summary>
        /// <param name="containerName">The name of the container from which to delete the file.</param>
        /// <param name="fileName">The file to delete.</param>
        public void Delete(string containerName, string fileName)
        {
            VerifyIsReady();

            // lock on the storage device so that only one storage operation can occur at a time
            lock (storageDevice)
            {
                // open a container
                using (StorageContainer currentContainer = OpenContainer(containerName))
                {
                    // attempt to delete the file
                    if (currentContainer.FileExists(fileName))
                    {
                        currentContainer.DeleteFile(fileName);
                    }
                }
            }
        }

        /// <summary>
        /// Determines if a given file exists.
        /// </summary>
        /// <param name="containerName">The name of the container in which to check for the file.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>True if the file exists, false otherwise.</returns>
        public bool FileExists(string containerName, string fileName)
        {
            VerifyIsReady();

            // lock on the storage device so that only one storage operation can occur at a time
            lock (storageDevice)
            {
                // open a container
                using (StorageContainer currentContainer = OpenContainer(containerName))
                {
                    return currentContainer.FileExists(fileName);
                }
            }
        }

        /// <summary>
        /// Gets an array of all files available in a container.
        /// </summary>
        /// <param name="containerName">The name of the container in which to search for files.</param>
        /// <returns>An array of file names of the files in the container.</returns>
        public string[] GetFiles(string containerName)
        {
            return GetFiles(containerName, null);
        }

        /// <summary>
        /// Gets an array of all files available in a container.
        /// </summary>
        /// <param name="containerName">The name of the container in which to search for files.</param>
        /// <param name="pattern">A search pattern to use to find files.</param>
        /// <returns>An array of file names of the files in the container.</returns>
        public string[] GetFiles(string containerName, string pattern)
        {
            VerifyIsReady();

            // lock on the storage device so that only one storage operation can occur at a time
            lock (storageDevice)
            {
                // open a container
                using (StorageContainer currentContainer = OpenContainer(containerName))
                {
                    return string.IsNullOrEmpty(pattern) ? currentContainer.GetFileNames() : currentContainer.GetFileNames(pattern);
                }
            }
        }
    }
}
