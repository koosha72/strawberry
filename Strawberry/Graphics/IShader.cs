namespace Strawberry.Graphics
{
    /// <summary>
    /// Represents a texture.
    /// </summary>
    public interface IShader : IBase
    {
        /// <summary>
        /// The graphics context by which the resource is created
        /// </summary>
        IGraphicsContext GraphicsContext { get; }

        /// <summary>
        /// Activates the shader for rendering
        /// </summary>
        void Activate();

        /// <summary>
        /// Sets a matrix parameter used inside shader
        /// </summary>
        /// <param name="constantBuffer">The name of constant buffer in which the matrix is available</param>
        /// <param name="variableName">The name of matrix variable inside shader</param>
        /// <param name="mat">The matrix value</param>
        /// <param name="transpose">If true the transposed version of matrix will be passed to the shader</param>
        void SetMatrixParameterByName(string constantBuffer, string variableName,
            Math.Matrix4 mat, bool transpose);
    }
}
