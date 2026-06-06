/*
 * Strawberry Game Engine
 * File: PostProcessEffect.cs
 * Author: Koosha Aabedini Nassab
 *
 * Represents a single post-processing effect with its own render target and shader.
 */

namespace Strawberry.Graphics
{
    /// <summary>
    /// Represents a single post-processing effect with its own render target and shader.
    /// </summary>
    public class PostProcessEffect
    {
        /// <summary>
        /// Gets the render target of this post-processing effect.
        /// </summary>
        public RenderTarget RenderTarget { get; private set; }
        /// <summary>
        /// Gets the graphics context that created and manages this post-processing effect.
        /// </summary>
        public IGraphicsContext GraphicsContext { get; set; }
        /// <summary>
        /// Gets the shader of this post-processing effect.
        /// </summary>
        public Shader Shader { get; set; }
        /// <summary>
        /// The name of the texture parameter in the shader that will be set to this effect's render target.
        /// </summary>
        public string TextureParameterName { get; private set; }
        /// <summary>
        /// Creates a new post-processing effect with the specified shader and texture parameter name.
        /// </summary>
        /// <param name="shader">The shader to use for this effect.</param>
        /// <param name="textureParameterName">The texture parameter name in the shader that will be set to this effect's render target.</param>
        public PostProcessEffect(Shader shader, string textureParameterName)
        {
            GraphicsContext = shader.GraphicsContext;
            RenderTarget = GraphicsContext.CreateRenderTarget(GraphicsContext.ActiveViewport.ScreenSize);
            TextureParameterName = textureParameterName;
        }
        /// <summary>
        /// Activates this effect's render target for rendering.
        /// </summary>
        public void Activate()
        {
            GraphicsContext.ActivateRenderTarget(RenderTarget);
        }
    }
}
