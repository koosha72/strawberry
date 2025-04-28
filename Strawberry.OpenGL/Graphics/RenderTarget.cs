using Strawberry.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Strawberry.OpenGL.Graphics
{
    public class RenderTarget : Base, IRenderTarget
    {
        Texture texture;

        int depthBuffer = 0;
        int frameBuffer = 0;

        public ITexture Texture
        {
            get { return texture; }
            private set { texture = (Texture)value; }
        }

        public int Width
        {
            get { return texture.Width; }
        }

        public int Height
        {
            get { return texture.Height; }
        }

        internal int GLFrameBuffer { get { return frameBuffer; } }

        GraphicsContext graphicsContext;
        public IGraphicsContext GraphicsContext { get { return graphicsContext; } }

        public RenderTarget(GraphicsContext gc, int width, int height)
        {
            texture = (Texture)gc.CreateTexture(width, height, (byte[])null);

            depthBuffer = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthBuffer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent16, width, height);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

            frameBuffer = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
                TextureTarget.Texture2D, texture.GLTexture, 0);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, depthBuffer);

            var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferErrorCode.FramebufferComplete)
            {
                throw new Exception("Framebuffer generation failed with status code: " + status);
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            graphicsContext = gc;
        }

        public void SetFilter(TextureFiltering minFilter, TextureFiltering magFilter)
        {
            Texture.SetFilter(minFilter, magFilter);
        }

        protected override void CleanManaged()
        {
            GL.DeleteRenderbuffer(depthBuffer);
            depthBuffer = 0;
            Texture.Dispose();
            GL.DeleteFramebuffer(frameBuffer);
            frameBuffer = 0;
            base.CleanManaged();
        }
    }
}
