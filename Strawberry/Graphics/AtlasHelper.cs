using System.Xml.Linq;
using Strawberry.Core;
using Strawberry.Graphics;
using Strawberry.Math;
using Strawberry.Platform;

namespace Strawberry.Graphics;

public static class AtlasHelper
{
    /// <summary>
    /// Loads sprites from a texture and a sprite map.
    /// </summary>
    /// <param name="gameContext">The current game context</param>
    /// <param name="texturePath">path to the texture file</param>
    /// <param name="spriteMap">path to the sprite map file</param>
    /// <param name="textureAssetName">Texture name in the asset manager</param>
    /// <param name="spriteAssetsPrefix">The prefix of loaded sprites in the asset manager</param>
    /// <param name="assets">The asset manager object, if null the gameContext.Assets will be used.</param>
    public static void LoadSprites(IGameContext gameContext, string texturePath, string spriteMap, string textureAssetName, string spriteAssetsPrefix, AssetManager assets = null)
    {
        var assetManager = assets == null ? gameContext.Assets : assets;
        var storage = PlatformServices.GetService<IStorage>();

        var texture = TextureLoader.Load(gameContext, texturePath);
        assetManager.Register<Texture>(textureAssetName, texture);

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

                var spr = new Sprite(texture, frameMap, size, size);
                assetManager.Register<Sprite>(spriteAssetsPrefix + name, spr);
            }
        }
    }
}