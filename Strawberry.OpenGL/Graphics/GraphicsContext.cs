using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Desktop;
using Strawberry.Graphics;
using Strawberry.Math;

namespace Strawberry.OpenGL.Graphics
{
    public class GraphicsContext : Base, IGraphicsContext
    {
        public Viewport ActiveViewport { get; private set; }

        public IShader ActiveShader { get; internal set; }

        RenderTarget renderTarget;

        public IRenderTarget ActiveRenderTarget
        {
            get { return renderTarget; }
        }

        OpenTK.Windowing.Desktop.GameWindow wnd = null;

        public ITexture PixelTexture
        {
            get;
            private set;
        }

        Dictionary<string, BlendState> blendStates;

        public void Initialize(object wnd, int width, int height, bool fullscreen)
        {
            blendStates = new Dictionary<string, BlendState>();

            if (wnd != null)
                this.wnd = (OpenTK.Windowing.Desktop.GameWindow)wnd;

            GL.Enable(EnableCap.Blend);

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
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public void Clear(Color color)
        {
            Clear(color.R, color.G, color.B, color.A);

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
                    state.RGBEquation = BlendEquationMode.FuncAdd;
                    break;
                case BlendEquation.Subtract:
                    state.RGBEquation = BlendEquationMode.FuncSubtract;
                    break;
            }

            switch (mode.AlphaEquation)
            {
                case BlendEquation.Add:
                    state.AlphaEquation = BlendEquationMode.FuncAdd;
                    break;
                case BlendEquation.Subtract:
                    state.AlphaEquation = BlendEquationMode.FuncSubtract;
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
            if (wnd != null)
            {
                GL.Viewport((int)viewport.ScreenPos.X, -(int)viewport.ScreenPos.Y,
                    (int)viewport.ScreenSize.X, (int)viewport.ScreenSize.Y);
            }
            else
            {
                GL.Viewport((int)viewport.ScreenPos.X, -(int)viewport.ScreenPos.Y,
                    (int)viewport.ScreenSize.X, (int)viewport.ScreenSize.Y);
            }
            this.ActiveViewport = viewport;
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

        public IShader CreateShader(string vsCode, string psCode, string vsEntryPoint,
                    string psEntryPoint, VertexElementContainer elements)
        {
            Shader shader = new Shader(this, vsCode, psCode, elements);

            return shader;
        }

        public IGeometry<T> CreateGeometry<T>(T[] vertices, uint[] indices, GeometryType vbType, GeometryType ibType) where T : struct
        {
            IGeometry<T> geo = new Geometry<T>(this, vertices, indices, vbType, ibType);

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
            var monitor = Monitors.GetPrimaryMonitor();
            return new Vector2(monitor.ClientArea.Size.X, monitor.ClientArea.Size.Y);
        }

        #region private methods
        BlendingFactorSrc ToSourcFactor(BlendFactor factor)
        {
            BlendingFactorSrc result = BlendingFactorSrc.One;

            switch (factor)
            {
                case BlendFactor.SrcAlpha:
                    result = BlendingFactorSrc.SrcAlpha;
                    break;
                case BlendFactor.InvSrcAlpha:
                    result = BlendingFactorSrc.OneMinusSrcAlpha;
                    break;
                case BlendFactor.One:
                    result = BlendingFactorSrc.One;
                    break;
                case BlendFactor.Zero:
                    result = BlendingFactorSrc.Zero;
                    break;
                case BlendFactor.SrcColor:
                    result = BlendingFactorSrc.SrcColor;
                    break;
                case BlendFactor.InvSrcColor:
                    result = BlendingFactorSrc.OneMinusSrcColor;
                    break;
            }

            return result;
        }


        BlendingFactorDest ToDestFactor(BlendFactor factor)
        {
            BlendingFactorDest result = BlendingFactorDest.One;

            switch (factor)
            {
                case BlendFactor.SrcAlpha:
                    result = BlendingFactorDest.SrcAlpha;
                    break;
                case BlendFactor.InvSrcAlpha:
                    result = BlendingFactorDest.OneMinusSrcAlpha;
                    break;
                case BlendFactor.One:
                    result = BlendingFactorDest.One;
                    break;
                case BlendFactor.Zero:
                    result = BlendingFactorDest.Zero;
                    break;
                case BlendFactor.SrcColor:
                    result = BlendingFactorDest.SrcColor;
                    break;
                case BlendFactor.InvSrcColor:
                    result = BlendingFactorDest.OneMinusSrcColor;
                    break;
            }

            return result;
        }

        public void ActivateRenderTarget(IRenderTarget renderTarget)
        {
            this.renderTarget = (RenderTarget)renderTarget;
            if (this.renderTarget == null)
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                return;
            }
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, this.renderTarget.GLFrameBuffer);
        }

        public IRenderTarget CreateRenderTarget(int width, int height)
        {
            return new RenderTarget(this, width, height);
        }

        public IRenderTarget CreateRenderTarget(Vector2 size)
        {
            return new RenderTarget(this, (int)size.X, (int)size.Y);
        }
        #endregion
    }
}
