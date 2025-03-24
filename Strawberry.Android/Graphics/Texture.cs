using Android.Graphics;
using Android.Opengl;
using Java.Nio;
using Strawberry.Graphics;
using Strawberry.Math;
using Color = Strawberry.Graphics.Color;

namespace Strawberry.Android.Graphics;

internal class Texture : Base, ITexture
{
    public int Width { get; private set; }

    public int Height { get; private set; }

    public int ActualWidth { get { return Width; } }

    public int ActualHeight { get { return Height; } }

    public Vector2 UVFactor { get { return new Vector2(1.0f, 1.0f); } }

    int texture = 0;

    internal int GLTexture { get { return texture; } }


    GraphicsContext graphicsContext;

    public IGraphicsContext GraphicsContext { get { return graphicsContext; } }

    int format;

    public Texture(GraphicsContext gc, int width, int height, Color[] data, TextureFormat format)
    {
        graphicsContext = gc;
        int[] textures = new int[1];
        GLES30.GlGenTextures(1, textures, 0);
        texture = textures[0];

        GLES30.GlBindTexture(GLES30.GlTexture2d, texture);
        GLES30.GlTexParameteri(GLES30.GlTexture2d, GLES30.GlTextureMinFilter, GLES30.GlLinear);
        GLES30.GlTexParameteri(GLES30.GlTexture2d, GLES30.GlTextureMagFilter, GLES30.GlLinear);
        GLES30.GlTexParameteri(GLES30.GlTexture2d, GLES30.GlTextureWrapS, GLES30.GlClampToEdge);
        GLES30.GlTexParameteri(GLES30.GlTexture2d, GLES30.GlTextureWrapT, GLES30.GlClampToEdge);
        int internalFormat = GLES30.GlRgba;
        int pformat = GLES30.GlRgba;
        switch (format)
        {
            case TextureFormat.R8G8B8A8:
                pformat = GLES30.GlRgba;
                internalFormat = GLES30.GlRgba;
                break;
            case TextureFormat.B8G8R8A8:
                throw new NotSupportedException();
                break;
            case TextureFormat.A:
                pformat = GLES30.GlAlpha;
                internalFormat = GLES30.GlAlpha;
                break;
        }
        FloatBuffer buffer = FloatBuffer.Allocate(data.Length * 4);
        foreach (var c in data)
        {
            buffer.Put(c.R);
            buffer.Put(c.G);
            buffer.Put(c.B);
            buffer.Put(c.A);
        }

        GLES30.GlTexImage2D(GLES30.GlTexture2d, 0, internalFormat,
            width, height, 0, pformat, GLES30.GlFloat, buffer);
        this.format = pformat;
        this.Width = width;
        this.Height = height;
    }

    public Texture(GraphicsContext gc, int width, int height, byte[] data, TextureFormat format)
    {
        graphicsContext = gc;

        int[] textures = new int[1];
        GLES30.GlGenTextures(1, textures, 0);
        texture = textures[0];

        GLES30.GlBindTexture(GLES30.GlTexture2d, texture);
        GLES30.GlTexParameteri(GLES30.GlTexture2d, GLES30.GlTextureMinFilter, GLES30.GlLinear);
        GLES30.GlTexParameteri(GLES30.GlTexture2d, GLES30.GlTextureMagFilter, GLES30.GlLinear);
        GLES30.GlTexParameteri(GLES30.GlTexture2d, GLES30.GlTextureWrapS, GLES30.GlClampToEdge);
        GLES30.GlTexParameteri(GLES30.GlTexture2d, GLES30.GlTextureWrapT, GLES30.GlClampToEdge);
        int internalFormat = GLES30.GlRgba;
        int pformat = GLES30.GlRgba;
        switch (format)
        {
            case TextureFormat.R8G8B8A8:
                pformat = GLES30.GlRgba;
                internalFormat = GLES30.GlRgba;
                break;
            case TextureFormat.B8G8R8A8:
                throw new NotSupportedException();
                break;
            case TextureFormat.A:
                pformat = GLES30.GlAlpha;
                internalFormat = GLES30.GlAlpha;
                break;
        }


        ByteBuffer buffer = ByteBuffer.Wrap(data, 0, data.Length);
        buffer.Order(ByteOrder.NativeOrder());

        GLES30.GlTexImage2D(GLES30.GlTexture2d, 0, internalFormat,
            width, height, 0, pformat, GLES30.GlUnsignedByte, buffer);

        this.format = pformat;
        this.Width = width;
        this.Height = height;
    }

    public void Activate(IShader shader, string name)
    {
        Shader s = (Shader)shader;
        GLES30.GlActiveTexture(GLES30.GlTexture0);
        GLES30.GlBindTexture(GLES30.GlTexture2d, texture);
        GLES30.GlUniform1i(GLES30.GlGetUniformLocation(s.Program, name), 0);
    }


    protected override void CleanManaged()
    {
        GLES30.GlDeleteTextures(1, new int[] { texture }, 0);
    }


    public void SetFilter(TextureFiltering minFilter, TextureFiltering magFilter)
    {
        int min = GLES30.GlLinear;
        if (minFilter == TextureFiltering.Nearest)
            min = GLES30.GlNearest;

        int mag = GLES30.GlLinear;
        if (magFilter == TextureFiltering.Nearest)
            mag = GLES30.GlNearest;

        GLES30.GlBindTexture(GLES30.GlTexture2d, texture);
        GLES30.GlTexParameteri(GLES30.GlTexture2d, GLES30.GlTextureMinFilter, min);
        GLES30.GlTexParameteri(GLES30.GlTexture2d, GLES30.GlTextureMagFilter, mag);
        GLES30.GlBindTexture(GLES30.GlTexture2d, 0);
    }

    public byte[] CopyToByteArray()
    {
        throw new NotImplementedException();
    }

    public void Update(byte[] data)
    {
        ByteBuffer buffer = ByteBuffer.Wrap(data, 0, data.Length);
        buffer.Order(ByteOrder.NativeOrder());
        GLES30.GlBindTexture(GLES30.GlTexture2d, texture);
        GLES30.GlTexSubImage2D(GLES30.GlTexture2d, 0, 0, 0, Width, Height, format, GLES30.GlUnsignedByte, buffer);
        GLES30.GlBindTexture(GLES30.GlTexture2d, 0);
    }
}
