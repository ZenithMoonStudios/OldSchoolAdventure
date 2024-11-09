using System.IO;

namespace EasyStorage
{
    /// <summary>
    /// A method for loading or saving a file.
    /// </summary>
    /// <param name="stream">A Stream to use for accessing the file data.</param>
    public delegate void FileAction(Stream stream);
}
