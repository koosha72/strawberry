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

        public PostProcessEffect(IShader shader)
        {
            GraphicsContext = shader.GraphicsContext;
            RenderTarget = GraphicsContext.CreateRenderTarget(GraphicsContext.ActiveViewport.ScreenSize);
        }

        public void Activate()
        {
            GraphicsContext.ActivateRenderTarget(RenderTarget);
        }
    }
}
