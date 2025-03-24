using Android.Opengl;
using Strawberry.Android.Helpers;
using Strawberry.Graphics;
using Strawberry.Math;

namespace Strawberry.Android.Graphics;

public class GraphicsContext : Base, IGraphicsContext
{
    public Viewport ActiveViewport => throw new NotImplementedException();

    public IShader ActiveShader { get; internal set; }

    public ITexture PixelTexture { get; private set; }

    public IRenderTarget ActiveRenderTarget => throw new NotImplementedException();

    public bool IsDisposed => throw new NotImplementedException();

    EGLHelper wnd = null;

    Dictionary<string, BlendState> blendStates;

    public void Initialize(object source, int width, int height, bool fullscreen)
    {
        blendStates = new Dictionary<string, BlendState>();

        if (wnd != null)
            this.wnd = (EGLHelper)source;

        GLES30.GlEnable(GLES30.GlBlend);

        AddBlendMode(new BlendMode
        {
            RGBSourceFactor = BlendFactor.SrcAlpha,
            RGBDestFactor = BlendFactor.InvSrcAlpha,
            AlphaSourceFactor = BlendFactor.SrcAlpha,
            AlphaDestFactor = BlendFactor.InvSrcAlpha,
            RGBEquation = BlendEquation.Add,
            AlphaEquation = BlendEquation.Add,
            Color = new Color()
        }, "Default");

        ActivateBlendMode("Default");

        PixelTexture = CreateTexture(1, 1, new Color[] { Color.White });
    }

    public void ActivateBlendMode(string name)
    {
        throw new NotImplementedException();
    }

    public void ActivateRenderTarget(IRenderTarget renderTarget)
    {
        throw new NotImplementedException();
    }

    public void AddBlendMode(BlendMode mode, string name)
    {
        throw new NotImplementedException();
    }

    public void BeginRender()
    {
        throw new NotImplementedException();
    }

    public void Clear(float r, float g, float b, float a)
    {
        throw new NotImplementedException();
    }

    public void Clear(Color color)
    {
        throw new NotImplementedException();
    }

    public IGeometry<T> CreateGeometry<T>(T[] vertices, uint[] indices, GeometryType vbType, GeometryType ibType) where T : struct
    {
        throw new NotImplementedException();
    }

    public IRenderTarget CreateRenderTarget(int width, int height)
    {
        throw new NotImplementedException();
    }

    public IRenderTarget CreateRenderTarget(Vector2 size)
    {
        throw new NotImplementedException();
    }

    public IShader CreateShader(string vsCode, string psCode, string vsEntryPoint, string psEntryPoint, VertexElementContainer elements)
    {
        throw new NotImplementedException();
    }

    public ITexture CreateTexture(int width, int height, Color[] data, TextureFormat format = TextureFormat.R8G8B8A8)
    {
        return new Texture(this, width, height, data, format);
    }

    public ITexture CreateTexture(int width, int height, byte[] data, TextureFormat format = TextureFormat.R8G8B8A8)
    {
        return new Texture(this, width, height, data, format);
    }

    public void EndRender()
    {
        throw new NotImplementedException();
    }

    public bool IsApplicationIdle()
    {
        throw new NotImplementedException();
    }

    public void Resize(int width, int height)
    {
        throw new NotImplementedException();
    }

    public void SetViewport(Viewport viewport)
    {
        throw new NotImplementedException();
    }
}
