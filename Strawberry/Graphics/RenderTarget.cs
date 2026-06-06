/*
 * Strawberry Game Engine
 * File: RenderTarget.cs
 * Author: Koosha Aabedini Nassab
 *
 * Abstract render target resource for render-to-texture operations.
 */

namespace Strawberry.Graphics
{
    public abstract class RenderTarget : DisposableReferenceObject
    {
        public abstract Texture Texture { get; }

        /// <summary>
        /// The graphics context by which the resource is created
        /// </summary>
        public abstract IGraphicsContext GraphicsContext { get; }

        /// <summary>
        /// Sets the filtering of the texture
        /// </summary>
        /// <param name="minFilter">Min filtering</param>
        /// <param name="magFilter">Mag filtering</param>
        public abstract void SetFilter(TextureFiltering minFilter, TextureFiltering magFilter);
    }
}
