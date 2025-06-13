using OpenTK.Graphics.OpenGL;
using Strawberry.Graphics;
using System.Runtime.InteropServices;

namespace Strawberry.Desktop.Graphics
{
    internal class Geometry<T> : Strawberry.Graphics.Geometry<T> where T : struct
    {
        int vbo;
        int ibo;
        int vao = 0;
        int size = 0;

        GraphicsContext graphicsContext;
        public override IGraphicsContext GraphicsContext { get { return graphicsContext; } }

        public int IndicesCout { get; private set; }

        public Geometry(IGraphicsContext context, T[] vertices, uint[] indices, GeometryType vbType, GeometryType ibType)
        {
            vbo = GL.GenBuffer();
            ibo = GL.GenBuffer();
            vao = GL.GenVertexArray();

            size = Marshal.SizeOf(typeof(T));
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);

            if (vbType == GeometryType.Static)
                GL.BufferData<T>(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * size), vertices, BufferUsageHint.StaticDraw);
            else
                GL.BufferData<T>(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * size), vertices, BufferUsageHint.StreamDraw);

            if (ibType == GeometryType.Static)
                GL.BufferData<uint>(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(uint)), indices, BufferUsageHint.StaticDraw);
            else
                GL.BufferData<uint>(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(uint)), indices, BufferUsageHint.StreamDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            this.graphicsContext = (GraphicsContext)context;
            this.IndicesCout = indices.Length;
        }

        public override void Render()
        {
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);
            ((Shader)graphicsContext.ActiveShader).ActivateLayout();

            GL.DrawElements(PrimitiveType.Triangles, this.IndicesCout, DrawElementsType.UnsignedInt, IntPtr.Zero);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            ((Shader)graphicsContext.ActiveShader).DeActivateLayout();
        }

        protected override void CleanManaged()
        {
            GL.DeleteBuffer(ibo);
            GL.DeleteBuffer(vbo);
        }


        public override void UpdateVB(T[] vertices)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData<T>(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * size), vertices, BufferUsageHint.StreamDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public override void UpdateIB(uint[] indices)
        {
            throw new NotImplementedException();
        }
    }
}
