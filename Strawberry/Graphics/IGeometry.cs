namespace Strawberry.Graphics
{
    /// <summary>
    /// Represents a geometry.
    /// </summary>
    public interface IGeometry<T> : IBase where T : struct
    {
        /// <summary>
        /// The graphics context by which the resource is created
        /// </summary>
        IGraphicsContext GraphicsContext { get; }

        /// <summary>
        /// Renders the geometry using the current active shader of graphics context
        /// </summary>
        void Render();

        /// <summary>
        /// Updates the vertex data of the geometry
        /// </summary>
        /// <param name="vertices">Verices to be used</param>
        void UpdateVB(T[] vertices);

        /// <summary>
        /// Updates the index data of the geometry
        /// </summary>
        /// <param name="indices">Indices to be used</param>
        void UpdateIB(uint[] indices);
    }
}
