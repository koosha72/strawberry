using System.Diagnostics;
using System.Runtime.InteropServices;
using Strawberry.Graphics;
using Strawberry.Web.Helpers;

namespace Strawberry.Web.Graphics;

internal class Geometry<T> : Base, IGeometry<T> where T : struct
{
    int vbo;
    int ibo;
    int vao = 0;
    int size = 0;

    GraphicsContext graphicsContext;
    public IGraphicsContext GraphicsContext { get { return graphicsContext; } }

    public int IndicesCount { get; private set; }

    public Geometry(IGraphicsContext context, T[] vertices, uint[] indices, GeometryType vbType, GeometryType ibType)
    {
        int[] buffers = new int[2];
        GL.GenBuffers(2, buffers);
        vbo = buffers[0];
        ibo = buffers[1];
        int[] vertexArrays = new int[1];
        GL.GenVertexArrays(1, vertexArrays);
        vao = vertexArrays[0];

        size = Marshal.SizeOf(typeof(T));

        GL.BindVertexArray(vao);
        GL.BindBuffer(GL.ArrayBuffer, vbo);
        GL.BindBuffer(GL.ElementArrayBuffer, ibo);

        byte[] verticesBytes = MemoryMarshal.AsBytes<T>(vertices).ToArray();

        unsafe
        {
            fixed (byte* p = verticesBytes)
            {
                IntPtr ptr = (IntPtr)p;

                if (vbType == GeometryType.Static)
                    GL.BufferData(GL.ArrayBuffer, vertices.Length * size, ptr, GL.StaticDraw);
                else
                    GL.BufferData(GL.ArrayBuffer, vertices.Length * size, ptr, GL.StreamDraw);
            }
        }

        byte[] indicesBytes = new byte[indices.Length * 4];
        System.Buffer.BlockCopy(indices, 0, indicesBytes, 0, indicesBytes.Length);

        unsafe
        {
            fixed (uint* p = indices)
            {
                GL.BufferData(
                    GL.ElementArrayBuffer,
                    indices.Length * sizeof(uint),
                    (IntPtr)p,
                    ibType == GeometryType.Static ? GL.StaticDraw : GL.StreamDraw
                );
            }
        }

        GL.BindBuffer(GL.ElementArrayBuffer, 0);
        GL.BindBuffer(GL.ArrayBuffer, 0);
        GL.BindVertexArray(0);
        this.graphicsContext = (GraphicsContext)context;
        this.IndicesCount = indices.Length;
    }

    public void Render()
    {
        GL.BindVertexArray(vao);
        GL.BindBuffer(GL.ArrayBuffer, vbo);
        GL.BindBuffer(GL.ElementArrayBuffer, ibo);
        ((Shader)graphicsContext.ActiveShader).ActivateLayout();

        GL.DrawElements(GL.Triangles, this.IndicesCount, GL.UnsignedInt, IntPtr.Zero);

        GL.BindBuffer(GL.ElementArrayBuffer, 0);
        GL.BindBuffer(GL.ArrayBuffer, 0);
        ((Shader)graphicsContext.ActiveShader).DeActivateLayout();
    }

    protected override void CleanManaged()
    {
        GL.DeleteBuffers(2, new int[] { ibo, vbo });
    }


    public void UpdateVB(T[] vertices)
    {
        GL.BindBuffer(GL.ArrayBuffer, vbo);
        byte[] verticesBytes = MemoryMarshal.AsBytes<T>(vertices).ToArray();

        unsafe
        {
            fixed (byte* p = verticesBytes)
            {
                IntPtr ptr = (IntPtr)p;

                GL.BufferData(GL.ArrayBuffer, vertices.Length * size, ptr, GL.StreamDraw);
            }
        }
        GL.BindBuffer(GL.ArrayBuffer, 0);
    }

    public void UpdateIB(uint[] indices)
    {
        throw new NotImplementedException();
    }
}