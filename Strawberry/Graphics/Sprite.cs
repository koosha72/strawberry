using Strawberry.Math;

namespace Strawberry.Graphics
{
    public class Sprite : ReferenceObject
    {
        public Texture Texture { get; set; }

        public int ImageCount { get; private set; }

        public Vector2 TopLeft { get; private set; }

        public Vector2 TexSize { get; private set; }

        public Vector2 Skip { get; private set; }

        public Vector2 Size { get; set; }

        public List<Vector2> FrameMap { get; set; }

        public Sprite(Texture texture, int imageCount, Vector2 size, Vector2 topLeft, Vector2 texSize, Vector2 skip)
        {
            this.Texture = texture;
            this.ImageCount = imageCount;
            this.TopLeft = topLeft;
            this.TexSize = texSize;
            this.Skip = skip;
            this.Size = size;
        }

        public Sprite(Texture texture, Vector2[] imageMap, Vector2 size, Vector2 texSize)
        {
            this.Texture = texture;
            this.ImageCount = imageMap.Length;
            this.TopLeft = new Vector2();
            this.TexSize = texSize;
            this.Skip = new Vector2();
            this.Size = size;
            FrameMap = new List<Vector2>();
            FrameMap.AddRange(imageMap);
        }

        public void ReInitialize(Texture texture, Vector2[] imageMap, Vector2 size, Vector2 texSize)
        {
            this.Texture = texture;
            this.ImageCount = imageMap.Length;
            this.TopLeft = new Vector2();
            this.TexSize = texSize;
            this.Skip = new Vector2();
            this.Size = size;
            FrameMap = new List<Vector2>();
            FrameMap.AddRange(imageMap);
        }
    }
}
