using System.Runtime.InteropServices;
using Android.Graphics;
using Android.Opengl;
using Java.Nio;
using Strawberry.Graphics;
using Strawberry.Math;
using Color = Strawberry.Graphics.Color;

namespace Strawberry.Android.Graphics;

internal class Texture : Strawberry.Graphics.Texture
{
    public override int Width { get; }

    public override int Height { get; }

    public override int ActualWidth { get { return Width; } }

    public override int ActualHeight { get { return Height; } }

    public override Vector2 UVFactor { get { return new Vector2(1.0f, 1.0f); } }

    public override TextureSettings TextureSettings { get; }

    int texture = 0;

    internal int GLTexture { get { return texture; } }


    GraphicsContext graphicsContext;

    public override IGraphicsContext GraphicsContext { get { return graphicsContext; } }

    int format;

    public Texture(GraphicsContext gc, int width, int height, Color[] data, TextureSettings settings)
    {
        graphicsContext = gc;
        int[] textures = new int[1];
        GLES30.GlGenTextures(1, textures, 0);
        texture = textures[0];

        int wrapS = GLES30.GlClampToEdge;
        int wrapT = GLES30.GlClampToEdge;
        switch (settings.WrapS)
        {
            case TextureWrap.Repeat:
                wrapS = GLES30.GlRepeat;
                break;
            case TextureWrap.ClampToEdge:
                wrapS = GLES30.GlClampToEdge;
                break;
            case TextureWrap.MirroredRepeat:
                wrapS = GLES30.GlMirroredRepeat;
                break;
        }

        switch (settings.WrapT)
        {
            case TextureWrap.Repeat:
                wrapT = GLES30.GlRepeat;
                break;
            case TextureWrap.ClampToEdge:
                wrapT = GLES30.GlClampToEdge;
                break;
            case TextureWrap.MirroredRepeat:
                wrapT = GLES30.GlMirroredRepeat;
                break;
        }

        int minFilter = GLES30.GlNearest;
        int magFilter = GLES30.GlNearest;

        switch (settings.MinFilter)
        {
            case TextureFiltering.Linear:
                minFilter = GLES30.GlLinear;
                break;
            case TextureFiltering.Nearest:
                minFilter = GLES30.GlNearest;
                break;
        }
        switch (settings.MagFilter)
        {
            case TextureFiltering.Linear:
                magFilter = GLES30.GlLinear;
                break;
            case TextureFiltering.Nearest:
                magFilter = GLES30.GlNearest;
                break;
        }

        GLES30.GlBindTexture(GLES30.GlTexture2d, texture);
        GLES30.GlTexParameteri(GLES30.GlTexture2d, GLES30.GlTextureMinFilter, minFilter);
        GLES30.GlTexParameteri(GLES30.GlTexture2d, GLES30.GlTextureMagFilter, magFilter);
        GLES30.GlTexParameteri(GLES30.GlTexture2d, GLES30.GlTextureWrapS, wrapS);
        GLES30.GlTexParameteri(GLES30.GlTexture2d, GLES30.GlTextureWrapT, wrapT);
        int internalFormat = GLES30.GlRgba;
        int pformat = GLES30.GlRgba;
        switch (settings.Format)
        {
            case TextureFormat.R8G8B8A8:
                pformat = GLES30.GlRgba;
                internalFormat = GLES30.GlRgba;
                break;
            case TextureFormat.B8G8R8A8:
                throw new NotSupportedException();
            case TextureFormat.A:
                pformat = GLES30.GlAlpha;
                internalFormat = GLES30.GlAlpha;
                break;
        }
        int pixelCount = width * height;
        byte[] colorsBytes = new byte[pixelCount * 4]; // 4 bytes per pixel (RGBA)

        for (int i = 0; i < pixelCount; i++)
        {
            // Convert float (0.0 - 1.0) to byte (0 - 255)
            colorsBytes[i * 4 + 0] = (byte)(data[i].R * 255f);
            colorsBytes[i * 4 + 1] = (byte)(data[i].G * 255f);
            colorsBytes[i * 4 + 2] = (byte)(data[i].B * 255f);
            colorsBytes[i * 4 + 3] = (byte)(data[i].A * 255f);
        }

        ByteBuffer colorsBuffer = ByteBuffer.Wrap(colorsBytes);

        GLES30.GlTexImage2D(GLES30.GlTexture2d, 0, internalFormat,
            width, height, 0, pformat, GLES30.GlUnsignedByte, colorsBuffer);
        format = pformat;
        Width = width;
        Height = height;

        TextureSettings = settings;
    }

    public Texture(GraphicsContext gc, int width, int height, byte[] data, TextureSettings settings)
    {
        graphicsContext = gc;

        int[] textures = new int[1];
        GLES30.GlGenTextures(1, textures, 0);
        texture = textures[0];
        int wrapS = GLES30.GlClampToEdge;
        int wrapT = GLES30.GlClampToEdge;
        switch (settings.WrapS)
        {
            case TextureWrap.Repeat:
                wrapS = GLES30.GlRepeat;
                break;
            case TextureWrap.ClampToEdge:
                wrapS = GLES30.GlClampToEdge;
                break;
            case TextureWrap.MirroredRepeat:
                wrapS = GLES30.GlMirroredRepeat;
                break;
        }

        switch (settings.WrapT)
        {
            case TextureWrap.Repeat:
                wrapT = GLES30.GlRepeat;
                break;
            case TextureWrap.ClampToEdge:
                wrapT = GLES30.GlClampToEdge;
                break;
            case TextureWrap.MirroredRepeat:
                wrapT = GLES30.GlMirroredRepeat;
                break;
        }

        int minFilter = GLES30.GlNearest;
        int magFilter = GLES30.GlNearest;

        switch (settings.MinFilter)
        {
            case TextureFiltering.Linear:
                minFilter = GLES30.GlLinear;
                break;
            case TextureFiltering.Nearest:
                minFilter = GLES30.GlNearest;
                break;
        }
        switch (settings.MagFilter)
        {
            case TextureFiltering.Linear:
                magFilter = GLES30.GlLinear;
                break;
            case TextureFiltering.Nearest:
                magFilter = GLES30.GlNearest;
                break;
        }

        GLES30.GlBindTexture(GLES30.GlTexture2d, texture);
        GLES30.GlTexParameteri(GLES30.GlTexture2d, GLES30.GlTextureMinFilter, minFilter);
        GLES30.GlTexParameteri(GLES30.GlTexture2d, GLES30.GlTextureMagFilter, magFilter);
        GLES30.GlTexParameteri(GLES30.GlTexture2d, GLES30.GlTextureWrapS, wrapS);
        GLES30.GlTexParameteri(GLES30.GlTexture2d, GLES30.GlTextureWrapT, wrapT);
        int internalFormat = GLES30.GlRgba;
        int pformat = GLES30.GlRgba;
        switch (settings.Format)
        {
            case TextureFormat.R8G8B8A8:
                pformat = GLES30.GlRgba;
                internalFormat = GLES30.GlRgba;
                break;
            case TextureFormat.B8G8R8A8:
                throw new NotSupportedException();
            case TextureFormat.A:
                pformat = GLES30.GlAlpha;
                internalFormat = GLES30.GlAlpha;
                break;
        }


        ByteBuffer buffer = ByteBuffer.Wrap(data, 0, data.Length);
        buffer.Order(ByteOrder.NativeOrder());

        GLES30.GlTexImage2D(GLES30.GlTexture2d, 0, internalFormat,
            width, height, 0, pformat, GLES30.GlUnsignedByte, buffer);

        format = pformat;
        Width = width;
        Height = height;
        TextureSettings = settings;
    }

    public override void Activate(Strawberry.Graphics.Shader shader, string name)
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


    public override void SetFilter(TextureFiltering minFilter, TextureFiltering magFilter)
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

    public override byte[] CopyToByteArray()
    {
        throw new NotImplementedException();
    }

    public override void Update(byte[] data)
    {
        ByteBuffer buffer = ByteBuffer.Wrap(data, 0, data.Length);
        buffer.Order(ByteOrder.NativeOrder());
        GLES30.GlBindTexture(GLES30.GlTexture2d, texture);
        GLES30.GlTexSubImage2D(GLES30.GlTexture2d, 0, 0, 0, Width, Height, format, GLES30.GlUnsignedByte, buffer);
        GLES30.GlBindTexture(GLES30.GlTexture2d, 0);
    }
}
