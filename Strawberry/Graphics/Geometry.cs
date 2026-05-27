namespace Strawberry.Graphics
{
    /// <summary>
    /// Represents a renderable geometric mesh composed of vertices and optional indices.
    /// </summary>
    /// <typeparam name="T">The vertex type defining the layout of the geometry's vertex data (e.g., position, color, texture coordinates). Must be a value type.</typeparam>
    public abstract class Geometry<T> : DisposableReferenceObject where T : struct
    {
        /// <summary>
        /// Gets the graphics context that created and manages this geometry resource.
        /// </summary>
        public abstract IGraphicsContext GraphicsContext { get; }

        /// <summary>
        /// Renders the geometry using the currently active shader and pipeline state of the graphics context.
        /// </summary>
        public abstract void Render();

        /// <summary>
        /// Updates the underlying vertex buffer with the provided vertex data.
        /// </summary>
        /// <param name="vertices">An array of vertices defining the new geometry shape and attributes.</param>
        public abstract void UpdateVB(T[] vertices);

        /// <summary>
        /// Updates the underlying index buffer with the provided index data.
        /// </summary>
        /// <param name="indices">An array of unsigned integers defining the order in which vertices should be rendered.</param>
        public abstract void UpdateIB(uint[] indices);
    }
}