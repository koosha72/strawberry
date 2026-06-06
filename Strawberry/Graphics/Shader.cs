/*
 * Strawberry Game Engine
 * File: Shader.cs
 * Author: Koosha Aabedini Nassab
 *
 * Base shader abstraction for GPU programs and parameter setting.
 */

namespace Strawberry.Graphics
{
    /// <summary>
    /// Represents a texture.
    /// </summary>
    public abstract class Shader : DisposableReferenceObject
    {
        /// <summary>
        /// Gets the graphics context by which the resource is created
        /// </summary>
        public abstract IGraphicsContext GraphicsContext { get; }

        /// <summary>
        /// Activates the shader for rendering
        /// </summary>
        public abstract void Activate();

        /// <summary>
        /// Sets a matrix parameter used inside shader
        /// </summary>
        /// <param name="constantBuffer">The name of constant buffer in which the matrix is available</param>
        /// <param name="variableName">The name of matrix variable inside shader</param>
        /// <param name="mat">The matrix value</param>
        /// <param name="transpose">If true the transposed version of matrix will be passed to the shader</param>
        public abstract void SetMatrixParameterByName(string constantBuffer, string variableName,
            Math.Matrix4 mat, bool transpose);
    }
}
