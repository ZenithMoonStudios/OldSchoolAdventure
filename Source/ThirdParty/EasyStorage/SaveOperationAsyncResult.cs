using System;
using System.IO;
using System.Threading;

namespace EasyStorage
{
    enum SaveOperation
    {
        Save,

    }

    class SaveOperationAsyncResult : IAsyncResult
    {
        private readonly object accessLock = new object();

        private bool isCompleted = false;

        private readonly StorageDevice storageDevice;
        private readonly string containerName;
        private readonly string fileName;
        private readonly FileAction fileAction;
        private readonly FileMode fileMode;

        public object AsyncState { get; set; }
        public WaitHandle AsyncWaitHandle { get; private set; }
        public bool CompletedSynchronously { get { return false; } }

        public bool IsCompleted
        {
            get
            {
                lock (accessLock)
                    return isCompleted;
            }
        }

        internal SaveOperationAsyncResult(StorageDevice device, string container, string file, FileAction action, FileMode mode)
        {
            this.storageDevice = device;
            this.containerName = container;
            this.fileName = file;
            this.fileAction = action;
            this.fileMode = mode;
        }

        private void EndOpenContainer(IAsyncResult result)
        {
            using (var container = storageDevice.EndOpenContainer(result))
            {
                if (fileMode == FileMode.Create)
                {
                }
                else if (fileMode == FileMode.Open)
                {
                }
            }

            lock (accessLock)
                isCompleted = true;
        }
    }
}
