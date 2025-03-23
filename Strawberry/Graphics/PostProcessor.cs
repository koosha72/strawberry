using Strawberry.Collection;
using Strawberry.Core;
using System;
using tainicom.Aether.Physics2D.Collision.Shapes;

namespace Strawberry.Graphics
{
    public class PostProcessor
    {
        OrderedDictionary<string, PostProcessEffect> effects;

        public IGraphicsContext GraphicsContext { get; private set; }

        IGeometry<VertexPositionTexColor> geometry;

        VertexPositionTexColor[] vertices;

        uint[] indices;


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

        public void Activate()
        {
            if(effects.Count == 0)
            {
                GraphicsContext.ActivateRenderTarget(null);
                return;
            }

            effects[0].Activate();
        }

        public void Render()
        {
            if (effects.Count == 0)
                return;
            int i = 1;
            for (i = 1; i < effects.Count; i++)
            {
                effects[i].Activate();
                effects[i - 1].Shader.Activate();
                effects[i - 1].RenderTarget.Texture.Activate(effects[i - 1].Shader, "tex0");
            }
            
            GraphicsContext.ActivateRenderTarget(null);
            effects[i - 1].Shader.Activate();
            effects[i - 1].RenderTarget.Texture.Activate(effects[i - 1].Shader, "tex0");
            geometry.Render();
        }

        public void AddPostProcessEffect(string name, PostProcessEffect effect)
        {
            effects.Add(name, effect);
        }

        public void RemovePostProcessEffect(string name)
        {
            effects.Remove(name);
        }
    }
}
