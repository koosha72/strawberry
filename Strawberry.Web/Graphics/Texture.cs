using System.Runtime.InteropServices;
using Strawberry.Graphics;
using Strawberry.Math;
using Strawberry.Web.Helpers;
using Color = Strawberry.Graphics.Color;

namespace Strawberry.Web.Graphics;

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
        GL.GenTextures(1, textures);
        texture = textures[0];
        GL.BindTexture(GL.Texture2D, texture);
        GL.TexParameteri(GL.Texture2D, GL.TextureMinFilter, GL.Linear);
        GL.TexParameteri(GL.Texture2D, GL.TextureMagFilter, GL.Linear);
        GL.TexParameteri(GL.Texture2D, GL.TextureWrapS, GL.ClampToEdge);
        GL.TexParameteri(GL.Texture2D, GL.TextureWrapT, GL.ClampToEdge);
        int internalFormat = GL.Rgba;
        int pformat = GL.Rgba;
        switch (format)
        {
            case TextureFormat.R8G8B8A8:
                pformat = GL.Rgba;
                internalFormat = GL.Rgba32f;
                break;
            case TextureFormat.B8G8R8A8:
                throw new NotSupportedException();
                break;
            case TextureFormat.A:
                pformat = GL.Alpha;
                internalFormat = GL.Alpha;
                break;
        }
        byte[] colorsBytes = MemoryMarshal.AsBytes<Color>(data.AsSpan()).ToArray();
        unsafe
        {
            fixed (byte* p = colorsBytes)
            {
                IntPtr ptr = (IntPtr)p;

                GL.TexImage2D(GL.Texture2D, 0, internalFormat,
                    width, height, 0, pformat, GL.Float, ptr);
            }
        }
        this.format = pformat;
        this.Width = width;
        this.Height = height;
    }

    public Texture(GraphicsContext gc, int width, int height, byte[] data, TextureFormat format)
    {
        graphicsContext = gc;

        int[] textures = new int[1];
        GL.GenTextures(1, textures);
        texture = textures[0];

        GL.BindTexture(GL.Texture2D, texture);
        GL.TexParameteri(GL.Texture2D, GL.TextureMinFilter, GL.Linear);
        GL.TexParameteri(GL.Texture2D, GL.TextureMagFilter, GL.Linear);
        GL.TexParameteri(GL.Texture2D, GL.TextureWrapS, GL.ClampToEdge);
        GL.TexParameteri(GL.Texture2D, GL.TextureWrapT, GL.ClampToEdge);
        int internalFormat = GL.Rgba;
        int pformat = GL.Rgba;
        switch (format)
        {
            case TextureFormat.R8G8B8A8:
                pformat = GL.Rgba;
                internalFormat = GL.Rgba;
                break;
            case TextureFormat.B8G8R8A8:
                throw new NotSupportedException();
                break;
            case TextureFormat.A:
                pformat = GL.Alpha;
                internalFormat = GL.Alpha;
                break;
        }


        unsafe
        {
            fixed (byte* p = data)
            {
                IntPtr ptr = (IntPtr)p;

                GL.TexImage2D(GL.Texture2D, 0, internalFormat,
                    width, height, 0, pformat, GL.UnsignedByte, ptr);
            }
        }

        this.format = pformat;
        this.Width = width;
        this.Height = height;
    }

    public void Activate(IShader shader, string name)
    {
        Shader s = (Shader)shader;
        GL.ActiveTexture(GL.Texture0);
        GL.BindTexture(GL.Texture2D, texture);
        GL.Uniform1(GL.GetUniformLocation(s.Program, name), 0);
    }


    protected override void CleanManaged()
    {
        GL.DeleteTextures(1, new int[] { texture });
    }


    public void SetFilter(TextureFiltering minFilter, TextureFiltering magFilter)
    {
        int min = GL.Linear;
        if (minFilter == TextureFiltering.Nearest)
            min = GL.Nearest;

        int mag = GL.Linear;
        if (magFilter == TextureFiltering.Nearest)
            mag = GL.Nearest;

        GL.BindTexture(GL.Texture2D, texture);
        GL.TexParameteri(GL.Texture2D, GL.TextureMinFilter, min);
        GL.TexParameteri(GL.Texture2D, GL.TextureMagFilter, mag);
        GL.BindTexture(GL.Texture2D, 0);
    }

    public byte[] CopyToByteArray()
    {
        throw new NotImplementedException();
    }

    public void Update(byte[] data)
    {
        GL.BindTexture(GL.Texture2D, texture);
        unsafe
        {
            fixed (byte* p = data)
            {
                IntPtr ptr = (IntPtr)p;

                GL.TexSubImage2D(GL.Texture2D, 0, 0, 0, Width, Height, format, GL.UnsignedByte, ptr);
            }
        }
        GL.BindTexture(GL.Texture2D, 0);
    }
}
