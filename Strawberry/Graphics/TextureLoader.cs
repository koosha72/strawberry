using Strawberry.Core;
using Strawberry.Platform;

namespace Strawberry.Graphics;


public static class TextureLoader
{
    static ushort texVersion = 1;

    public static Texture Load(IGameContext gameContext, string path)
    {
        var storage = PlatformServices.GetService<IStorage>();

        MemoryStream mem = new MemoryStream(storage.ReadAllBytes(path));
        BinaryReader reader = new BinaryReader(mem);

        ushort ver = reader.ReadUInt16();
        if (ver > texVersion)
            throw new IOException("The version of texture file is newer than this loader.");
        int width = reader.ReadInt32();
        int height = reader.ReadInt32();
        byte[] colors = reader.ReadBytes(width * height * 4);

        reader.Close();
        mem.Dispose();

        return gameContext.GraphicsContext.CreateTexture(width, height, colors);
    }
}