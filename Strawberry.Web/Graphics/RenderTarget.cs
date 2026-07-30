using Strawberry.Graphics;
using Strawberry.Web.Helpers;

namespace Strawberry.Web.Graphics;

public class RenderTarget : Strawberry.Graphics.RenderTarget
{
    Texture texture;

    int depthBuffer = 0;
    int frameBuffer = 0;

    public override Strawberry.Graphics.Texture Texture
    {
        get { return texture; }
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
    public override IGraphicsContext GraphicsContext { get { return graphicsContext; } }

    public RenderTarget(GraphicsContext gc, int width, int height)
    {
        texture = (Texture)gc.CreateTexture(width, height, (byte[])null);

        int[] dbs = new int[1];
        GL.GenRenderbuffers(1, dbs);
        depthBuffer = dbs[0];
        GL.BindRenderbuffer(GL.Renderbuffer, depthBuffer);
        GL.RenderbufferStorage(GL.Renderbuffer, GL.DepthComponent16, width, height);
        GL.BindRenderbuffer(GL.Renderbuffer, 0);

        int[] fbs = new int[1];
        GL.GenFramebuffers(1, fbs);
        frameBuffer = fbs[0];
        GL.BindFramebuffer(GL.Framebuffer, frameBuffer);
        GL.FramebufferTexture2D(GL.Framebuffer, GL.ColorAttachment0,
            GL.Texture2D, texture.GLTexture, 0);
        GL.FramebufferRenderbuffer(GL.Framebuffer, GL.DepthAttachment, GL.Renderbuffer, depthBuffer);

        var status = GL.CheckFramebufferStatus(GL.Framebuffer);
        if (status != GL.FramebufferComplete)
        {
            throw new Exception("Framebuffer generation failed with status code: " + status);
        }

        GL.BindFramebuffer(GL.Framebuffer, 0);
        graphicsContext = gc;
    }

    public override void SetFilter(TextureFiltering minFilter, TextureFiltering magFilter)
    {
        Texture.SetFilter(minFilter, magFilter);
    }

    protected override void CleanUnmanaged()
    {
        GL.DeleteRenderbuffers(1, new int[] { depthBuffer });
        depthBuffer = 0;
        Texture.Dispose();
        GL.DeleteFramebuffers(1, new int[] { frameBuffer });
        frameBuffer = 0;
        base.CleanUnmanaged();
    }
}