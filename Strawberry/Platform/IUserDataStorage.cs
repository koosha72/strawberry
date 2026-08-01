/*
 * Strawberry Game Engine
 * File: IStorage.cs
 * Author: Koosha Aabedini Nassab
 *
 * Interface for platform-independent user data file storage access.
 */

namespace Strawberry.Platform;

/// <summary>
/// The storage service that reads and writes user data files in a platform-independent way.
/// </summary>
public interface IUserDataStorage : IPlatformService
{
    /// <summary>
    /// Opens a read-only stream to the specified file.
    /// </summary>
    /// <param name="path">Path to the file.</param>
    /// <returns>A read-only stream to the specified file.</returns>
    /// <exception cref="FileNotFoundException">Thrown if the file does not exist.</exception>
    Stream Open(string path);

    /// <summary>
    /// Reads all the bytes in the specified file.
    /// </summary>
    /// <param name="path">Path to the file.</param>
    /// <returns>Bytes in the specified file.</returns>
    /// <exception cref="FileNotFoundException">Thrown if the file does not exist.</exception>
    byte[] ReadAllBytes(string path);

    /// <summary>
    /// Lists all files (not directories) in the specified directory. Non-recursive.
    /// </summary>
    /// <param name="path">Path to the directory.</param>
    /// <returns>An array of file paths in the specified directory.</returns>
    string[] List(string path);

    /// <summary>
    /// Opens a stream for writing to the specified file, creating the file and any 
    /// missing parent directories if necessary.
    /// </summary>
    /// <param name="path">Path to the file to write to.</param>
    /// <param name="append">If true, appends to the end of the file if it already exists; otherwise overwrites.</param>
    /// <returns>A writable stream to the specified file.</returns>
    Stream Write(string path, bool append = false);

    /// <summary>
    /// Checks if a file exists at the specified path. Returns false for directories.
    /// </summary>
    /// <param name="path">Path to the file.</param>
    /// <returns>True if a file exists at the specified path, false otherwise.</returns>
    bool Exists(string path);

    /// <summary>
    /// Deletes the file at the specified path. Does nothing if the file does not exist.
    /// </summary>
    /// <param name="path">Path to the file to be deleted.</param>
    void Delete(string path);

    /// <summary>
    /// Creates a directory at the specified path. Does nothing if the directory already exists.
    /// </summary>
    /// <param name="path">Path to the directory to be created.</param>
    void CreateDirectory(string path);
}