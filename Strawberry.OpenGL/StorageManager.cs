using Strawberry.Misc;

namespace Strawberry.OpenGL;

public class StorageManager : IStorage
{
    public string[] List(string path)
    {
        return Directory.GetFiles(path);
    }

    public Stream Open(string path)
    {
        return File.Open(path, FileMode.Open);
    }

    public byte[] ReadAllBytes(string path)
    {
        return File.ReadAllBytes(path);
    }
}
