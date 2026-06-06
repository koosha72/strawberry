namespace Strawberry.Misc;
/// <summary>
/// The storage manager that handles file storage on different platforms and allows you to read data from a file system.
/// </summary>
public interface IStorage
{
    /// <summary>
    /// Opens a stream to the specified file.
    /// </summary>
    /// <param name="path">Path to the file.</param>
    /// <returns>A stream to the specified file.</returns>
    Stream Open(string path);
    /// <summary>
    /// Reads all the bytes in the specified file
    /// </summary>
    /// <param name="path">Path to the file.</param>
    /// <returns>Bytes in the specified file</returns>
    byte[] ReadAllBytes(string path);
    /// <summary>
    /// Lists all files in the specified directory.
    /// </summary>
    /// <param name="path">Path to the directory.</param>
    /// <returns>An array of files in the specified directory.</returns>
    public string[] List(string path);

}
