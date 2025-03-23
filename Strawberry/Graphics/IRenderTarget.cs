namespace Strawberry.Graphics
{
    public interface IRenderTarget : IBase
    {
        public ITexture Texture { get; }

        /// <summary>
        /// The graphics context by which the resource is created
        /// </summary>
        IGraphicsContext GraphicsContext { get; }

        /// <summary>
        /// Sets the filtering of the texture
        /// </summary>
        /// <param name="minFilter">Min filtering</param>
        /// <param name="magFilter">Mag filtering</param>
        void SetFilter(TextureFiltering minFilter, TextureFiltering magFilter);
    }
}
