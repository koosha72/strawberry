/*
 * Strawberry Game Engine
 * File: StrawberryShader.cs
 * Author: Koosha Aabedini Nassab
 *
 * Helper for compiling and managing platform shaders and shader helpers.
 * It will soon be modified heavily
 */

using Strawberry.Math;
using System.Text.RegularExpressions;

namespace Strawberry.Graphics
{
    /// <summary>
    /// A helper class for compiling and managing platform shaders
    /// </summary>
    public class StrawberryShader : Base, IDisposable
    {
        public Shader BaseShader { get; private set; }

        public string VsEntryPoint { get; private set; }

        public string PsEntryPoint { get; private set; }

        public IGraphicsContext GraphicsContext { get; private set; }

        public StrawberryShader(IGraphicsContext context)
        {
            this.GraphicsContext = context;
        }
        /// <summary>
        /// Initialize the shader with a vertex and pixel shader code
        /// </summary>
        /// <param name="vsCode">Vertex shader code</param>
        /// <param name="psCode">Pixel shader code</param>
        /// <param name="vsEntryPoint">Entry point for the vertex shader</param>
        /// <param name="psEntryPoint">Entry point for the pixel shader</param>
        /// <param name="elements">Shader input elements</param>
        /// <param name="lang">Shader language. HLSL is not supported right now.</param>
        public void Initialize(string vsCode, string psCode,
            string vsEntryPoint, string psEntryPoint, VertexElementContainer elements, ShaderLanguage lang = ShaderLanguage.GLSL)
        {
            string vs = "";
            string ps = "";

            if (lang == ShaderLanguage.HLSL)
            {
                Match m = Regex.Match(vsCode, "<HLSL>([^<]*)</HLSL>");
                vs = m.Value;
                vs = vs.Replace("<HLSL>", "");
                vs = vs.Replace("</HLSL>", "");

                m = Regex.Match(psCode, "<HLSL>([^<]*)</HLSL>");
                ps = m.Value;
                ps = ps.Replace("<HLSL>", "");
                ps = ps.Replace("</HLSL>", "");
            }

            if (lang == ShaderLanguage.GLSL)
            {
                Match m = Regex.Match(vsCode, "<GLSL>([^<]*)</GLSL>");
                vs = m.Value;
                vs = vs.Replace("<GLSL>", "");
                vs = vs.Replace("</GLSL>", "");

                m = Regex.Match(psCode, "<GLSL>([^<]*)</GLSL>");
                ps = m.Value;
                ps = ps.Replace("<GLSL>", "");
                ps = ps.Replace("</GLSL>", "");
            }

            BaseShader = GraphicsContext.CreateShader(vs, ps, vsEntryPoint, psEntryPoint, elements);
        }

        /// <summary>
        /// Activates the shader for rendering
        /// </summary>
        public virtual void Activate()
        {
            BaseShader.Activate();
        }

        protected override void CleanUnmanaged()
        {
            BaseShader.Dispose();
        }
    }

    /// <summary>
    /// Represents a simple vertex with 2-dimensional position and 4-byte color data.
    /// </summary>
    public struct VertexPositionColor
    {
        public Vector2 Position { get; set; }

        public Color Color { get; set; }


        public VertexPositionColor(Vector2 pos, Color color)
            : this()
        {
            this.Position = pos;
            this.Color = color;
        }
    }

    /// <summary>
    /// Represents a simple vertex with 2-dimensional position and texture coordinates data, and a 4-byte color data.
    /// </summary>
    public struct VertexPositionTexColor
    {
        public Vector2 Position { get; set; }

        public Vector2 TexCoord { get; set; }

        public Color Color { get; set; }


        public VertexPositionTexColor(Vector2 pos, Vector2 texCoord, Color color)
            : this()
        {
            this.Position = pos;
            this.Color = color;
            this.TexCoord = texCoord;
        }

        public VertexPositionTexColor(Vector4 pos, Color color)
            : this()
        {
            this.Position = new Math.Vector2(pos.X, pos.Y);
            this.Color = color;
            this.TexCoord = new Math.Vector2(pos.Z, pos.W);
        }
    }
}
