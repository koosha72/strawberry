using Strawberry.Math;

namespace Strawberry.Graphics
{
    /// <summary>
    /// Represents a graphics context used for rendering.
    /// </summary>
    public interface IGraphicsContext : IBase
    {
        /// <summary>
        /// The current viewport used for rendering
        /// </summary>
        Viewport ActiveViewport { get; }
        /// <summary>
        /// Represents a single pixel with white color. You can use it to draw rectangles, lines, etc.
        /// </summary>
        ITexture PixelTexture { get; }

        /// <summary>
        /// Gets the currently active render target.
        /// </summary>
        IRenderTarget ActiveRenderTarget { get; }

        /// <summary>
        /// Initializes the graphics context for rendering.
        /// </summary>
        /// <param name="source">The source to render on (It can be a window handle or a GameWindow object for OpenTK)</param>
        /// <param name="width">The width of the rendering area</param>
        /// <param name="height">The height of the rendering area</param>
        /// <param name="fullscreen">If true the resolution of the screen will be changed.</param>
        void Initialize(object source, int width, int height, bool fullscreen);

        /// <summary>
        /// Starts the rendering process
        /// </summary>
        void BeginRender();

        /// <summary>
        /// Cleares the screen to the specified color.
        /// </summary>
        /// <param name="r">The amount of red (between 0.0 to 1.0)</param>
        /// <param name="g">The amount of green (between 0.0 to 1.0)</param>
        /// <param name="b">The amount of blue (between 0.0 to 1.0)</param>
        /// <param name="a">The amount of transparency (between 0.0 to 1.0, 1.0 = opaque 0.0 = completly transparent)</param>
        void Clear(float r, float g, float b, float a);

        /// <summary>
        /// Cleares the screen to the specified color.
        /// </summary>
        /// <param name="color">Color</param>
        void Clear(Color color);

        /// <summary>
        /// Finalizes the rendering process.
        /// </summary>
        void EndRender();

        /// <summary>
        /// Adds a new blending mode to the graphics context.
        /// </summary>
        /// <param name="mode">The blend mode to use</param>
        /// <param name="name">The name of the added blend mode</param>
        void AddBlendMode(BlendMode mode, string name);

        /// <summary>
        /// Activates a blend mode for rendering
        /// </summary>
        /// <param name="name">The name by which the blend mode is added. (See AddBlendMode)</param>
        void ActivateBlendMode(string name);

        /// <summary>
        /// Activates a viewport for rendering
        /// </summary>
        /// <param name="viewport">The viewport</param>
        void SetViewport(Viewport viewport);

        /// <summary>
        /// Activates renderTarget to be used for rendering.
        /// </summary>
        /// <param name="renderTarget">The render target object. If null will switch to default render target.</param>
        void ActivateRenderTarget(IRenderTarget renderTarget);

        bool IsApplicationIdle();

        /// <summary>
        /// Creates a texture using an array of colors
        /// </summary>
        /// <param name="width">The width of the texture</param>
        /// <param name="height">The height of the texture</param>
        /// <param name="data">The color data (This data depends on Texture format)</param>
        /// <param name="format">The format the texture will use</param>
        /// <returns>The texture object</returns>
        ITexture CreateTexture(int width, int height, Color[] data, TextureFormat format = TextureFormat.R8G8B8A8);

        /// <summary>
        /// Creates a texture using an array of bytes
        /// </summary>
        /// <param name="width">The width of the texture</param>
        /// <param name="height">The height of the texture</param>
        /// <param name="data">The color data (This data depends on Texture format)</param>
        /// <param name="format">The format the texture will use</param>
        /// <returns></returns>
        ITexture CreateTexture(int width, int height, byte[] data, TextureFormat format = TextureFormat.R8G8B8A8);

        /// <summary>
        /// Creates a shader using vertex and pixel shaders passed to it
        /// </summary>
        /// <param name="vsCode">The vertex shader code</param>
        /// <param name="psCode">The pixel shader code</param>
        /// <param name="vsEntryPoint">The vertex shader main function</param>
        /// <param name="psEntryPoint">The pixel shader main function</param>
        /// <param name="elements">The elements to be passed to the shader like positions, colors, normals, etc</param>
        /// <returns></returns>
        IShader CreateShader(string vsCode, string psCode, string vsEntryPoint,
            string psEntryPoint, VertexElementContainer elements);

        /// <summary>
        /// Creates a geometry using specified vertext type.
        /// </summary>
        /// <typeparam name="T">The vertex type to be used, The data in the vertex should match VertexElementContainer used by shader</typeparam>
        /// <param name="vertices">An array of vertices</param>
        /// <param name="indices">An array of indices used to join vertices and make geometries</param>
        /// <param name="vbType">Type of vertex buffer (Dynamic or Static)</param>
        /// <param name="ibType">Type of index buffer (Dynamic or Static)</param>
        /// <returns></returns>
        IGeometry<T> CreateGeometry<T>(T[] vertices, uint[] indices, GeometryType vbType, GeometryType ibType) where T : struct;

        /// <summary>
        /// Creates a render target. It can be used for render to texture, etc.
        /// </summary>
        /// <param name="width">Width of the render target</param>
        /// <param name="height">Height of the render target</param>
        /// <returns></returns>
        IRenderTarget CreateRenderTarget(int width, int height);

        IRenderTarget CreateRenderTarget(Vector2 size);

        /// <summary>
        /// Resizes the main render target used by the graphics context
        /// </summary>
        /// <param name="width">New width</param>
        /// <param name="height">New height</param>
        void Resize(int width, int height);

        Vector2 GetScreenSize();
    }
}
