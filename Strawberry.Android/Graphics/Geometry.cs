using System.Diagnostics;
using System.Runtime.InteropServices;
using Android.Opengl;
using Java.Nio;
using Strawberry.Graphics;

namespace Strawberry.Android.Graphics;

public class Geometry<T> : Strawberry.Graphics.Geometry<T> where T : struct
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
        int[] buffers = new int[2];
        GLES30.GlGenBuffers(2, buffers, 0);
        vbo = buffers[0];
        ibo = buffers[1];
        int[] vertexArrays = new int[1];
        GLES30.GlGenVertexArrays(1, vertexArrays, 0);
        vao = vertexArrays[0];

        size = Marshal.SizeOf(typeof(T));
        GLES30.GlBindVertexArray(vao);
        GLES30.GlBindBuffer(GLES30.GlArrayBuffer, vbo);
        GLES30.GlBindBuffer(GLES30.GlElementArrayBuffer, ibo);

        byte[] verticesBytes = MemoryMarshal.AsBytes<T>(vertices).ToArray();

        ByteBuffer verticesBuffer = ByteBuffer.Wrap(verticesBytes);

        if (vbType == GeometryType.Static)
            GLES30.GlBufferData(GLES30.GlArrayBuffer, vertices.Length * size, verticesBuffer, GLES30.GlStaticDraw);
        else
            GLES30.GlBufferData(GLES30.GlArrayBuffer, vertices.Length * size, verticesBuffer, GLES30.GlStreamDraw);

        byte[] indicesBytes = new byte[indices.Length * 4];
        System.Buffer.BlockCopy(indices, 0, indicesBytes, 0, indicesBytes.Length);

        ByteBuffer indicesBuffer = ByteBuffer.Wrap(indicesBytes);

        if (ibType == GeometryType.Static)
            GLES30.GlBufferData(GLES30.GlElementArrayBuffer, indices.Length * sizeof(uint), indicesBuffer, GLES30.GlStaticDraw);
        else
            GLES30.GlBufferData(GLES30.GlElementArrayBuffer, indices.Length * sizeof(uint), indicesBuffer, GLES30.GlStreamDraw);

        GLES30.GlBindBuffer(GLES30.GlElementArrayBuffer, 0);
        GLES30.GlBindBuffer(GLES30.GlArrayBuffer, 0);

        this.graphicsContext = (GraphicsContext)context;
        this.IndicesCout = indices.Length;
    }

    public override void Render()
    {
        GLES30.GlBindVertexArray(vao);
        GLES30.GlBindBuffer(GLES30.GlArrayBuffer, vbo);
        GLES30.GlBindBuffer(GLES30.GlElementArrayBuffer, ibo);
        ((Shader)graphicsContext.ActiveShader).ActivateLayout();

        GLES30.GlDrawElements(GLES30.GlTriangles, this.IndicesCout, GLES30.GlUnsignedInt, 0);

        GLES30.GlBindBuffer(GLES30.GlElementArrayBuffer, 0);
        GLES30.GlBindBuffer(GLES30.GlArrayBuffer, 0);
        ((Shader)graphicsContext.ActiveShader).DeActivateLayout();
    }

    protected override void CleanManaged()
    {
        GLES30.GlDeleteBuffers(2, new int[] { ibo, vbo }, 0);
    }


    public override void UpdateVB(T[] vertices)
    {
        GLES30.GlBindBuffer(GLES30.GlArrayBuffer, vbo);
        byte[] verticesBytes = MemoryMarshal.AsBytes<T>(vertices).ToArray();

        ByteBuffer verticesBuffer = ByteBuffer.Wrap(verticesBytes);

        GLES30.GlBufferData(GLES30.GlArrayBuffer, vertices.Length * size, verticesBuffer, GLES30.GlStreamDraw);
        GLES30.GlBindBuffer(GLES30.GlArrayBuffer, 0);
    }

    public override void UpdateIB(uint[] indices)
    {
        throw new NotImplementedException();
    }
}