using Android.Opengl;
using Strawberry.Graphics;

namespace Strawberry.Android.Graphics;

internal class RenderTarget : Strawberry.Graphics.RenderTarget
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
        GLES30.GlGenRenderbuffers(1, dbs, 0);
        depthBuffer = dbs[0];
        GLES30.GlBindRenderbuffer(GLES30.GlRenderbuffer, depthBuffer);
        GLES30.GlRenderbufferStorage(GLES30.GlRenderbuffer, GLES30.GlDepthComponent16, width, height);
        GLES30.GlBindRenderbuffer(GLES30.GlRenderbuffer, 0);

        int[] fbs = new int[1];
        GLES30.GlGenFramebuffers(1, fbs, 0);
        frameBuffer = fbs[0];
        GLES30.GlBindFramebuffer(GLES30.GlFramebuffer, frameBuffer);
        GLES30.GlFramebufferTexture2D(GLES30.GlFramebuffer, GLES30.GlColorAttachment0,
            GLES30.GlTexture2d, texture.GLTexture, 0);
        GLES30.GlFramebufferRenderbuffer(GLES30.GlFramebuffer, GLES30.GlDepthAttachment, GLES30.GlRenderbuffer, depthBuffer);

        var status = GLES30.GlCheckFramebufferStatus(GLES30.GlFramebuffer);
        if (status != GLES30.GlFramebufferComplete)
        {
            throw new Exception("Framebuffer generation failed with status code: " + status);
        }

        GLES30.GlBindFramebuffer(GLES30.GlFramebuffer, 0);
        graphicsContext = gc;
    }

    public override void SetFilter(TextureFiltering minFilter, TextureFiltering magFilter)
    {
        Texture.SetFilter(minFilter, magFilter);
    }

    protected override void CleanManaged()
    {
        GLES30.GlDeleteRenderbuffers(1, new int[] { depthBuffer }, 0);
        depthBuffer = 0;
        Texture.Dispose();
        GLES30.GlDeleteFramebuffers(1, new int[] { frameBuffer }, 0);
        frameBuffer = 0;
        base.CleanManaged();
    }
}