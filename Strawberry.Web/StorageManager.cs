using System;
using Strawberry.Platform;

namespace Strawberry.Web;

public class StorageManager : IStorage
{
    public string RootUrl { get; set; }
    Dictionary<string, byte[]> cache = new Dictionary<string, byte[]>();

    public Stream Open(string path)
    {
        return new MemoryStream(cache[path]);
    }

    public byte[] ReadAllBytes(string path)
    {
        return cache[path];
    }

    public string[] List(string path)
    {
        return cache.Keys.ToArray();
    }

    public async Task AOTDownload(string path)
    {
        if (RootUrl == null)
            return;

        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri(RootUrl);
        var response = await client.GetAsync("Assets/" + path);
        var data = await response.Content.ReadAsByteArrayAsync();
        cache.Add(path, data);
    }
}
