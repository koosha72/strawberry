namespace Strawberry.Graphics
{
    public class PostProcessEffect
    {
        public RenderTarget RenderTarget { get; private set; }
        public IGraphicsContext GraphicsContext { get; set; }
        public Shader Shader { get; set; }

        public string TextureParameterName { get; private set; }

        public PostProcessEffect(Shader shader, string textureParameterName)
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
