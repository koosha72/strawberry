using Android.Opengl;
using Strawberry.Android.Helpers;
using Strawberry.Graphics;
using Strawberry.Math;

namespace Strawberry.Android.Graphics;

public class GraphicsContext : Base, IGraphicsContext
{
    public Viewport ActiveViewport { get; private set; }

    public IShader ActiveShader { get; internal set; }

    public ITexture PixelTexture { get; private set; }

    RenderTarget renderTarget;

    public IRenderTarget ActiveRenderTarget
    {
        get { return renderTarget; }
    }

    EGLHelper wnd = null;

    Dictionary<string, BlendState> blendStates;

    public void Initialize(object source, int width, int height, bool fullscreen)
    {
        blendStates = new Dictionary<string, BlendState>();

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
        BlendState state = blendStates[name];

        GLES30.GlBlendColor(state.Color.R, state.Color.G, state.Color.B, state.Color.A);
        GLES30.GlBlendFuncSeparate(state.RGBSource, state.RGBDest, state.AlphaSource, state.AlphaDest);
        GLES30.GlBlendEquationSeparate(state.RGBEquation, state.AlphaEquation);
    }

    public void ActivateRenderTarget(IRenderTarget renderTarget)
    {
        this.renderTarget = (RenderTarget)renderTarget;
        if (this.renderTarget == null)
        {
            GLES30.GlBindFramebuffer(GLES30.GlFramebuffer, 0);
            return;
        }
        GLES30.GlBindFramebuffer(GLES30.GlFramebuffer, this.renderTarget.GLFrameBuffer);
    }

    public void AddBlendMode(BlendMode mode, string name)
    {
        BlendState state = new BlendState();

        state.Color = mode.Color;

        state.RGBSource = ToGlBlending(mode.RGBSourceFactor);

        state.RGBDest = ToGlBlending(mode.RGBDestFactor);

        state.AlphaSource = ToGlBlending(mode.AlphaSourceFactor);

        state.AlphaDest = ToGlBlending(mode.AlphaDestFactor);

        switch (mode.RGBEquation)
        {
            case BlendEquation.Add:
                state.RGBEquation = GLES30.GlFuncAdd;
                break;
            case BlendEquation.Subtract:
                state.RGBEquation = GLES30.GlFuncSubtract;
                break;
        }

        switch (mode.AlphaEquation)
        {
            case BlendEquation.Add:
                state.AlphaEquation = GLES30.GlFuncAdd;
                break;
            case BlendEquation.Subtract:
                state.AlphaEquation = GLES30.GlFuncSubtract;
                break;
        }

        blendStates.Add(name, state);
    }

    public void BeginRender()
    {
    }

    public void Clear(float r, float g, float b, float a)
    {
        GLES30.GlClearColor(r, g, b, a);
        GLES30.GlClear(GLES30.GlColorBufferBit | GLES30.GlDepthBufferBit);
    }

    public void Clear(Color color)
    {
        Clear(color.R, color.G, color.B, color.A);
    }

    public IGeometry<T> CreateGeometry<T>(T[] vertices, uint[] indices, GeometryType vbType, GeometryType ibType) where T : struct
    {
        IGeometry<T> geo = new Geometry<T>(this, vertices, indices, vbType, ibType);

        return geo;
    }

    public IRenderTarget CreateRenderTarget(int width, int height)
    {
        return new RenderTarget(this, width, height);
    }

    public IRenderTarget CreateRenderTarget(Vector2 size)
    {
        return CreateRenderTarget((int)size.X, (int)size.Y);
    }

    public IShader CreateShader(string vsCode, string psCode, string vsEntryPoint, string psEntryPoint, VertexElementContainer elements)
    {
        Shader shader = new Shader(this, vsCode, psCode, elements);

        return shader;
    }

    public ITexture CreateTexture(int width, int height, Color[] data, TextureFormat format = TextureFormat.R8G8B8A8)
    {
        return new Texture(this, width, height, data, new TextureSettings
        {
            Format = format
        });
    }

    public ITexture CreateTexture(int width, int height, byte[] data, TextureFormat format = TextureFormat.R8G8B8A8)
    {
        return new Texture(this, width, height, data, new TextureSettings
        {
            Format = format
        });
    }

    public ITexture CreateTexture(int width, int height, Color[] data, TextureSettings settings)
    {
        return new Texture(this, width, height, data, settings);
    }

    public ITexture CreateTexture(int width, int height, byte[] data, TextureSettings settings)
    {
        return new Texture(this, width, height, data, settings);
    }

    public void EndRender()
    {
        if (wnd != null)
            wnd.SwapBuffers();
    }

    public bool IsApplicationIdle()
    {
        return false;
    }

    public Vector2 GetScreenSize()
    {
        if (wnd != null)
        {
            return wnd.GetScreenSize();
        }
        return Vector2.Zero;
    }


    public void Resize(int width, int height)
    {
        throw new NotImplementedException();
    }

    public void SetViewport(Viewport viewport)
    {
        if (wnd != null)
        {
            GLES30.GlViewport((int)viewport.ScreenPos.X, -(int)viewport.ScreenPos.Y,
                (int)viewport.ScreenSize.X, (int)viewport.ScreenSize.Y);
        }
        else
        {
            GLES30.GlViewport((int)viewport.ScreenPos.X, -(int)viewport.ScreenPos.Y,
                (int)viewport.ScreenSize.X, (int)viewport.ScreenSize.Y);
        }
        this.ActiveViewport = viewport;
    }

    int ToGlBlending(BlendFactor factor)
    {
        int result = GLES30.GlOne;

        switch (factor)
        {
            case BlendFactor.SrcAlpha:
                result = GLES30.GlSrcAlpha;
                break;
            case BlendFactor.InvSrcAlpha:
                result = GLES30.GlOneMinusSrcAlpha;
                break;
            case BlendFactor.One:
                result = GLES30.GlOne;
                break;
            case BlendFactor.Zero:
                result = GLES30.GlZero;
                break;
            case BlendFactor.SrcColor:
                result = GLES30.GlSrcColor;
                break;
            case BlendFactor.InvSrcColor:
                result = GLES30.GlOneMinusSrcColor;
                break;
        }

        return result;
    }
}
