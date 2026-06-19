using System.Xml.Linq;
using Strawberry.Core;
using Strawberry.Graphics;
using Strawberry.Math;
using Strawberry.Platform;

namespace Strawberry.Graphics;

public class AtlasHelper
{
    static ushort texVersion = 1;

    public Dictionary<string, Sprite> Sprites { get; } = new Dictionary<string, Sprite>();

    public Texture Texture
    {
        get;
        private set;
    }

    public Sprite this[string key]
    {
        get => Sprites[key];
    }

    public void LoadSprites(IGameContext gameContext, string texture, string spriteMap)
    {
        var storage = PlatformServices.GetService<IStorage>();

        MemoryStream mem = new MemoryStream(storage.ReadAllBytes(texture));
        BinaryReader reader = new BinaryReader(mem);

        ushort ver = reader.ReadUInt16();
        if (ver > texVersion)
            throw new IOException("The version of texture file is newer than this loader.");
        int width = reader.ReadInt32();
        int height = reader.ReadInt32();
        byte[] colors = reader.ReadBytes(width * height * 4);

        reader.Close();
        mem.Dispose();

        Texture = gameContext.GraphicsContext.CreateTexture(width, height, colors);
        using (MemoryStream mem2 = new MemoryStream(storage.ReadAllBytes(spriteMap)))
        {
            XDocument doc = XDocument.Load(mem2);
            var sprites = doc.Descendants("Sprite").ToList();

            foreach (var sprite in sprites)
            {
                var size = new Vector2(float.Parse(sprite.Attribute("width").Value), float.Parse(sprite.Attribute("height").Value));
                var name = sprite.Attribute("name").Value;

                Vector2[] frameMap = new Vector2[int.Parse(sprite.Attribute("frames_count").Value)];
                var frames = sprite.Descendants("Frame").ToList();
                for (int i = 0; i < frames.Count; i++)
                {
                    var frame = frames[i];
                    var x = float.Parse(frame.Attribute("left").Value);
                    var y = float.Parse(frame.Attribute("top").Value);
                    frameMap[i] = new Vector2(x, y);
                }

                var spr = new Sprite(Texture, frameMap, size, size);
                Sprites.Add(name, spr);
            }
        }
    }
}