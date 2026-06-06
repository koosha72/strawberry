/*
 * Strawberry Game Engine
 * File: Sprite.cs
 * Author: Koosha Aabedini Nassab
 *
 * Represents a 2D sprite with texture mapping and frame information.
 */

using Strawberry.Math;

namespace Strawberry.Graphics
{
    /// <summary>
    /// Represents a 2D sprite with texture mapping and frame information.
    /// </summary>
    public class Sprite : ReferenceObject
    {
        // Gets or sets the position of the sprite in 2D space
        public Vector2 Position { get; set; }
        /// <summary>
        /// Gets or sets texture that this sprite is using for rendering.
        /// </summary>
        public Texture Texture { get; set; }

        /// <summary>
        /// Gets the number of images or frames in the sprite sheet.
        /// </summary>
        public int ImageCount { get; private set; }

        /// <summary>
        /// Gets the top-left corner coordinates of the sprite within the texture.
        /// </summary>
        public Vector2 TopLeft { get; private set; }
        /// <summary>
        /// Gets the width and height of the sprite within the texture.
        /// </summary>
        public Vector2 TexSize { get; private set; }
        /// <summary>
        /// Gets the number of pixels to skip between each frame in the sprite sheet
        /// </summary>
        public Vector2 Skip { get; private set; }
        /// <summary>
        /// Gets or sets actual size of the sprite in 2D space
        /// </summary>
        public Vector2 Size { get; set; }
        /// <summary>
        /// Gets the positions of each frame in the sprite sheet, relative to the top-left corner
        /// </summary>
        public List<Vector2> FrameMap { get; set; }

        public Sprite(Texture texture, int imageCount, Vector2 size, Vector2 topLeft, Vector2 texSize, Vector2 skip)
        {
            Texture = texture;
            ImageCount = imageCount;
            TopLeft = topLeft;
            TexSize = texSize;
            Skip = skip;
            Size = size;
        }

        public Sprite(Texture texture, Vector2[] imageMap, Vector2 size, Vector2 texSize)
        {
            Texture = texture;
            ImageCount = imageMap.Length;
            TopLeft = new Vector2();
            TexSize = texSize;
            Skip = new Vector2();
            Size = size;
            FrameMap = new List<Vector2>();
            FrameMap.AddRange(imageMap);
        }

        public void ReInitialize(Texture texture, Vector2[] imageMap, Vector2 size, Vector2 texSize)
        {
            Texture = texture;
            ImageCount = imageMap.Length;
            TopLeft = new Vector2();
            TexSize = texSize;
            Skip = new Vector2();
            Size = size;
            FrameMap = new List<Vector2>();
            FrameMap.AddRange(imageMap);
        }
    }
}
