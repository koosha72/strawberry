/*
 * Strawberry Game Engine
 * File: PostProcessor.cs
 * Author: Koosha Aabedini Nassab
 *
 * Manages chained post-processing effects applied to render targets.
 */

using Strawberry.Collection;

namespace Strawberry.Graphics
{
    /// <summary>
    /// Manages chained post-processing effects applied to render targets.
    /// </summary>
    public class PostProcessor
    {
        OrderedDictionary<string, PostProcessEffect> effects;

        public IGraphicsContext GraphicsContext { get; private set; }

        Geometry<VertexPositionTexColor> geometry;

        VertexPositionTexColor[] vertices;

        uint[] indices;

        /// <summary>
        /// Creates a new post-processor with the specified graphics context.
        /// </summary>
        /// <param name="graphicsContext">The graphics context to use for rendering.</param>
        public PostProcessor(IGraphicsContext graphicsContext)
        {
            effects = new OrderedDictionary<string, PostProcessEffect>();
            GraphicsContext = graphicsContext;

            vertices = new VertexPositionTexColor[4];
            indices = new uint[6];

            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 2;
            indices[3] = 2;
            indices[4] = 3;
            indices[5] = 0;

            geometry = GraphicsContext.CreateGeometry<VertexPositionTexColor>(vertices, indices,
                GeometryType.Static, GeometryType.Static);
        }
        /// <summary>
        /// Activates the first effect in the post-processor and renders it to the screen.
        /// </summary>
        public void Activate()
        {
            if (effects.Count == 0)
            {
                GraphicsContext.ActivateRenderTarget(null);
                return;
            }

            effects[0].Activate();
        }
        /// <summary>
        /// Starts the post-processing effect chain.
        /// </summary>
        public void Render()
        {
            if (effects.Count == 0)
                return;
            int i = 1;
            for (i = 1; i < effects.Count; i++)
            {
                effects[i].Activate();
                effects[i - 1].Shader.Activate();
                effects[i - 1].RenderTarget.Texture.Activate(effects[i - 1].Shader, effects[i - 1].TextureParameterName);
            }

            GraphicsContext.ActivateRenderTarget(null);
            effects[i - 1].Shader.Activate();
            effects[i - 1].RenderTarget.Texture.Activate(effects[i - 1].Shader, effects[i - 1].TextureParameterName);
            geometry.Render();
        }

        /// <summary>
        /// Adds a post process effect to the list of effects
        /// </summary>
        /// <param name="name">Name of the post process effect</param>
        /// <param name="effect">Effect to add</param>
        public void AddPostProcessEffect(string name, PostProcessEffect effect)
        {
            effects.Add(name, effect);
        }
        /// <summary>
        /// Removes a post process effect from the list of effects
        /// </summary>
        /// <param name="name">Name of the post process effect</param>
        public void RemovePostProcessEffect(string name)
        {
            effects.Remove(name);
        }
    }
}
