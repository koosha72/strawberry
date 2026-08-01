using Android.Content.Res;
using Strawberry.Platform;
using App = Android.App;

namespace Strawberry.Android;

/// <summary>
/// The storage manager for android platform.
/// </summary>
public class AssetStorageManager : IAssetStorage
{
    AssetManager assetManager;
    public AssetStorageManager()
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
