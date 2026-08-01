/*
 * Strawberry Game Engine
 * File: IStorage.cs
 * Author: Koosha Aabedini Nassab
 *
 * Interface for platform-independent file storage access.
 */

namespace Strawberry.Platform;
/// <summary>
/// The storage service that reads game asset files.
/// </summary>
public interface IAssetStorage : IPlatformService
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
