using Strawberry.Graphics;
using Strawberry.Math;
using Strawberry.Web.Helpers;

namespace Strawberry.Web.Graphics
{
    public class GraphicsContext : Base, IGraphicsContext
    {
        public Viewport ActiveViewport { get; private set; }

        public Strawberry.Graphics.Shader ActiveShader { get; internal set; }

        RenderTarget renderTarget;

        public Strawberry.Graphics.RenderTarget ActiveRenderTarget
        {
            get { return renderTarget; }
        }

        public Strawberry.Graphics.Texture PixelTexture
        {
            get;
            private set;
        }

        EGLDisplayHolder wnd;

        Dictionary<string, BlendState> blendStates;

        public void Initialize(object wnd, int width, int height)
        {
            blendStates = new Dictionary<string, BlendState>();

            this.wnd = (EGLDisplayHolder)wnd;

            GL.Enable(GL.Blend);

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

            PixelTexture = this.CreateTexture(1, 1, new Color[] { Color.White });
        }

        public void BeginRender()
        {

        }

        public void Clear(float r, float g, float b, float a)
        {
            GL.ClearColor(r, g, b, a);
            GL.Clear(GL.ColorBufferBit | GL.DepthBufferBit);
        }

        public void Clear(Color color)
        {
            Clear(color.R, color.G, color.B, color.A);

        }

        public void EndRender()
        {
            //EGL.SwapBuffers(wnd.Display, wnd.Surface);
        }

        public bool IsApplicationIdle()
        {
            return false;
        }

        public void AddBlendMode(BlendMode mode, string name)
        {
            BlendState state = new BlendState();

            state.Color = mode.Color;

            state.RGBSource = ToSourcFactor(mode.RGBSourceFactor);

            state.RGBDest = ToDestFactor(mode.RGBDestFactor);

            state.AlphaSource = ToSourcFactor(mode.AlphaSourceFactor);

            state.AlphaDest = ToDestFactor(mode.AlphaDestFactor);

            switch (mode.RGBEquation)
            {
                case BlendEquation.Add:
                    state.RGBEquation = GL.FuncAdd;
                    break;
                case BlendEquation.Subtract:
                    state.RGBEquation = GL.FuncSubtract;
                    break;
            }

            switch (mode.AlphaEquation)
            {
                case BlendEquation.Add:
                    state.AlphaEquation = GL.FuncAdd;
                    break;
                case BlendEquation.Subtract:
                    state.AlphaEquation = GL.FuncSubtract;
                    break;
            }

            blendStates.Add(name, state);
        }

        public void ActivateBlendMode(string name)
        {
            BlendState state = blendStates[name];

            GL.BlendColor(state.Color.R, state.Color.G, state.Color.B, state.Color.A);
            GL.BlendFuncSeparate(state.RGBSource, state.RGBDest, state.AlphaSource, state.AlphaDest);
            GL.BlendEquationSeparate(state.RGBEquation, state.AlphaEquation);
        }

        public void SetViewport(Viewport viewport)
        {
            var s = ToGLViewport(GetScreenSize().Y, viewport);
            GL.Viewport(s.x, s.y, s.width, s.height);
            this.ActiveViewport = viewport;
        }

        (int x, int y, int width, int height) ToGLViewport(float displayHeight, Viewport viewport)
        {
            int x = (int)viewport.ScreenPos.X;
            int y = (int)(displayHeight - viewport.ScreenPos.Y - viewport.ScreenSize.Y);
            int w = (int)viewport.ScreenSize.X;
            int h = (int)viewport.ScreenSize.Y;
            return (x, y, w, h);
        }

        public Strawberry.Graphics.Texture CreateTexture(int width, int height, Color[] data, TextureFormat format = TextureFormat.R8G8B8A8)
        {
            return new Texture(this, width, height, data, new TextureSettings
            {
                Format = format
            });
        }

        public Strawberry.Graphics.Texture CreateTexture(int width, int height, byte[] data, TextureFormat format = TextureFormat.R8G8B8A8)
        {
            return new Texture(this, width, height, data, new TextureSettings
            {
                Format = format
            });
        }

        public Strawberry.Graphics.Texture CreateTexture(int width, int height, Color[] data, TextureSettings settings)
        {
            return new Texture(this, width, height, data, settings);
        }

        public Strawberry.Graphics.Texture CreateTexture(int width, int height, byte[] data, TextureSettings settings)
        {
            return new Texture(this, width, height, data, settings);
        }

        public Strawberry.Graphics.Shader CreateShader(string vsCode, string psCode, string vsEntryPoint,
                    string psEntryPoint, VertexElementContainer elements)
        {
            Shader shader = new Shader(this, vsCode, psCode, elements);

            return shader;
        }

        public Strawberry.Graphics.Geometry<T> CreateGeometry<T>(T[] vertices, uint[] indices, GeometryType vbType, GeometryType ibType) where T : struct
        {
            Strawberry.Graphics.Geometry<T> geo = new Geometry<T>(this, vertices, indices, vbType, ibType);

            return geo;
        }

        public void Resize(int width, int height)
        {
            throw new NotImplementedException();
        }

        protected override void CleanUnmanaged()
        {
            //DisplayDevice.Default.RestoreResolution();
        }

        public Vector2 GetScreenSize()
        {
            return wnd.GetCanvasSize();
        }

        #region private methods
        int ToSourcFactor(BlendFactor factor)
        {
            int result = GL.One;

            switch (factor)
            {
                case BlendFactor.SrcAlpha:
                    result = GL.SrcAlpha;
                    break;
                case BlendFactor.InvSrcAlpha:
                    result = GL.OneMinusSrcAlpha;
                    break;
                case BlendFactor.One:
                    result = GL.One;
                    break;
                case BlendFactor.Zero:
                    result = GL.Zero;
                    break;
                case BlendFactor.SrcColor:
                    result = GL.SrcColor;
                    break;
                case BlendFactor.InvSrcColor:
                    result = GL.OneMinusSrcColor;
                    break;
            }

            return result;
        }


        int ToDestFactor(BlendFactor factor)
        {
            int result = GL.One;

            switch (factor)
            {
                case BlendFactor.SrcAlpha:
                    result = GL.SrcAlpha;
                    break;
                case BlendFactor.InvSrcAlpha:
                    result = GL.OneMinusSrcAlpha;
                    break;
                case BlendFactor.One:
                    result = GL.One;
                    break;
                case BlendFactor.Zero:
                    result = GL.Zero;
                    break;
                case BlendFactor.SrcColor:
                    result = GL.SrcColor;
                    break;
                case BlendFactor.InvSrcColor:
                    result = GL.OneMinusSrcColor;
                    break;
            }

            return result;
        }

        public void ActivateRenderTarget(Strawberry.Graphics.RenderTarget renderTarget)
        {
            this.renderTarget = (RenderTarget)renderTarget;
            if (this.renderTarget == null)
            {
                GL.BindFramebuffer(GL.Framebuffer, 0);
                return;
            }
            GL.BindFramebuffer(GL.Framebuffer, this.renderTarget.GLFrameBuffer);
        }

        public Strawberry.Graphics.RenderTarget CreateRenderTarget(int width, int height)
        {
            return new RenderTarget(this, width, height);
        }

        public Strawberry.Graphics.RenderTarget CreateRenderTarget(Vector2 size)
        {
            return new RenderTarget(this, (int)size.X, (int)size.Y);
        }
        #endregion
    }
}
