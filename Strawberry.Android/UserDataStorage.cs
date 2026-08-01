
using Strawberry.Platform;

namespace Strawberry.Android;

public class UserDataStorage : IUserDataStorage
{
    private readonly string dataPath;
    public UserDataStorage()
    {
        dataPath = Application.Context.FilesDir.AbsolutePath;
    }

    public Stream Open(string path)
    {
        return File.Open(Path.Combine(dataPath, path), FileMode.Open);
    }

    public byte[] ReadAllBytes(string path)
    {
        return File.ReadAllBytes(Path.Combine(dataPath, path));
    }

    public string[] List(string path)
    {
        string fullDirPath = Path.Combine(dataPath, path);
        if (!Directory.Exists(fullDirPath)) return Array.Empty<string>();

        string[] files = Directory.GetFiles(fullDirPath);
        for (int i = 0; i < files.Length; i++)
        {
            files[i] = files[i].Substring(fullDirPath.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }
        return files;
    }

    public Stream Write(string path, bool append = false)
    {
        string fullPath = Path.Combine(dataPath, path);
        string dir = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
        return new FileStream(fullPath, append ? FileMode.Append : FileMode.Create);
    }

    public void Delete(string path)
    {
        File.Delete(Path.Combine(dataPath, path));
    }

    public bool Exists(string path)
    {
        return File.Exists(Path.Combine(dataPath, path));
    }

    public void CreateDirectory(string path)
    {
        Directory.CreateDirectory(Path.Combine(dataPath, path));
    }
}
