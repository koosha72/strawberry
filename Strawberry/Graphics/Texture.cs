namespace Strawberry.Graphics
{
    /// <summary>
    /// Represents a texture.
    /// </summary>
    public abstract class Texture : DisposableReferenceObject
    {
        /// <summary>
        /// Width of the texture.
        /// </summary>
        public abstract int Width { get; }
        /// <summary>
        /// Height of the texture.
        /// </summary>
        public abstract int Height { get; }

        /// <summary>
        /// The actual width of the texture (In Android the texture dimmensions should be a power of 2 so the width and actual width may be different)
        /// </summary>
        public abstract int ActualWidth { get; }
        /// The actual width of the texture (In Android the texture dimmensions should be a power of 2 so the height and actual height may be different)
        public abstract int ActualHeight { get; }

        public abstract Math.Vector2 UVFactor { get; }
        /// <summary>
        /// The graphics context by which the resource is created
        /// </summary>
        public abstract IGraphicsContext GraphicsContext { get; }

        public abstract TextureSettings TextureSettings { get; }

        /// <summary>
        /// Activates the texture to be used by the given shader
        /// </summary>
        /// <param name="shader">Shader by which the texture will be used</param>
        /// <param name="name">The name of the parameter in the shader code</param>
        public abstract void Activate(Shader shader, string name);

        /// <summary>
        /// Sets the filtering of the texture
        /// </summary>
        /// <param name="minFilter">Min filtering</param>
        /// <param name="magFilter">Mag filtering</param>
        public abstract void SetFilter(TextureFiltering minFilter, TextureFiltering magFilter);

        /// <summary>
        /// Copies the texture data to an array of bytes
        /// </summary>
        /// <returns>Texture data</returns>
        public abstract byte[] CopyToByteArray();

        /// <summary>
        /// Updates the texture using the given data.
        /// </summary>
        /// <param name="data">The rgba data to update the texture using with</param>
        public abstract void Update(byte[] data);
    }
}
