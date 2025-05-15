namespace Strawberry.Graphics
{
    /// <summary>
    /// Represents a geometry.
    /// </summary>
    public abstract class Geometry<T> : DisposableReferenceObject where T : struct
    {
        /// <summary>
        /// The graphics context by which the resource is created
        /// </summary>
        public abstract IGraphicsContext GraphicsContext { get; }

        /// <summary>
        /// Renders the geometry using the current active shader of graphics context
        /// </summary>
        public abstract void Render();

        /// <summary>
        /// Updates the vertex data of the geometry
        /// </summary>
        /// <param name="vertices">Verices to be used</param>
        public abstract void UpdateVB(T[] vertices);

        /// <summary>
        /// Updates the index data of the geometry
        /// </summary>
        /// <param name="indices">Indices to be used</param>
        public abstract void UpdateIB(uint[] indices);
    }
}
