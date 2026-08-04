using System.Runtime.InteropServices;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using Strawberry.Platform;

namespace Strawberry.Web;

public class UserDataStorage : IUserDataStorage
{
    private static Dictionary<string, byte[]> cache = new();
    private static bool initialized = false;

    public static void InitializeFromJson(string json)
    {
        cache = ParseAllFilesJson(json);
        initialized = true;
    }

    private static void EnsureInitialized()
    {
        if (!initialized)
            throw new InvalidOperationException(
                "UserDataStorage used before SetUserDataCache was called. " +
                "Ensure the JS initialize() function has completed.");
    }


    private static Dictionary<string, byte[]> ParseAllFilesJson(string json)
    {
        var result = new Dictionary<string, byte[]>();
        if (string.IsNullOrEmpty(json) || json == "{}") return result;

        using var doc = System.Text.Json.JsonDocument.Parse(json);
        foreach (var prop in doc.RootElement.EnumerateObject())
            result[prop.Name] = Convert.FromBase64String(prop.Value.GetString());
        return result;
    }

    public Stream Open(string path)
    {
        EnsureInitialized();
        if (!cache.TryGetValue(path, out var data))
            throw new FileNotFoundException($"File not found: {path}", path);
        return new MemoryStream(data, writable: false);
    }

    public byte[] ReadAllBytes(string path)
    {
        if (!cache.TryGetValue(path, out var data))
            throw new FileNotFoundException($"File not found: {path}", path);
        return data;
    }

    public string[] List(string path)
    {
        EnsureInitialized();
        return cache.Keys
            .Where(k => k.StartsWith(path, StringComparison.OrdinalIgnoreCase))
            .Select(k => k.Substring(path.Length).TrimStart('/'))
            .ToArray();
    }

    public Stream Write(string path, bool append = false)
    {
        EnsureInitialized();
        return new WebWriteStream(path, this, append);
    }

    internal void PersistWrite(string path, byte[] data, bool append)
    {
        if (append && cache.TryGetValue(path, out var existing))
        {
            var combined = new byte[existing.Length + data.Length];
            Buffer.BlockCopy(existing, 0, combined, 0, existing.Length);
            Buffer.BlockCopy(data, 0, combined, existing.Length, data.Length);
            cache[path] = combined;
            data = combined;
        }
        else
        {
            cache[path] = data;
        }

        // Fire-and-forget the async JS write
        _ = JSStorage.WriteFile(path, Convert.ToBase64String(data));
    }

    public void Delete(string path)
    {
        EnsureInitialized();
        cache.Remove(path);
        _ = JSStorage.DeleteFile(path);
    }

    public bool Exists(string path)
    {
        EnsureInitialized();
        return cache.ContainsKey(path);
    }

    public void CreateDirectory(string path)
    {
        // No-op on Web — directories are implicit in the path
    }

    private class WebWriteStream : MemoryStream
    {
        private readonly string path;
        private readonly UserDataStorage storage;
        private readonly bool append;
        private bool flushed = false;

        public WebWriteStream(string path, UserDataStorage storage, bool append)
        {
            this.path = path;
            this.storage = storage;
            this.append = append;
        }

        protected override void Dispose(bool disposing)
        {
            if (!flushed && disposing)
            {
                flushed = true;
                storage.PersistWrite(path, ToArray(), append);
            }
            base.Dispose(disposing);
        }
    }
}


internal static partial class JSStorage
{

    [JSImport("storage.write_file", "strawberry.js")]
    public static partial Task WriteFile(string path, string base64Data);

    [JSImport("storage.delete_file", "strawberry.js")]
    public static partial Task DeleteFile(string path);

    [JSImport("set_game_name", "strawberry.js")]
    public static partial void SetGameName(string name);
}