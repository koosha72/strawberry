using Android.Content.Res;
using Strawberry.Misc;
using App = Android.App;

namespace Strawberry.Android;

public class StorageManager : IStorage
{
    AssetManager assetManager;
    public StorageManager()
    {
        assetManager = App.Application.Context.Assets;
    }

    public string[] List(string path)
    {
        return assetManager.List(path);
    }

    public Stream Open(string path)
    {
        var stream = assetManager.Open(path);
        MemoryStream ms = new MemoryStream();
        stream.CopyTo(ms);
        ms.Seek(0, SeekOrigin.Begin);
        return ms;
    }

    public byte[] ReadAllBytes(string path)
    {
        using (var stream = assetManager.Open(path))
        {
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
