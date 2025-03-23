using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strawberry.Graphics
{
    public class PostProcessEffect
    {
        public IRenderTarget RenderTarget { get; private set; }
        public IGraphicsContext GraphicsContext { get; set; }
        public IShader Shader { get; set; }

        public string TextureParameterName { get; private set; }

        public PostProcessEffect(IShader shader, string textureParameterName)
        {
            GraphicsContext = shader.GraphicsContext;
            RenderTarget = GraphicsContext.CreateRenderTarget(GraphicsContext.ActiveViewport.ScreenSize);
            TextureParameterName = textureParameterName;
        }

        public void Activate()
        {
            GraphicsContext.ActivateRenderTarget(RenderTarget);
        }
    }
}
