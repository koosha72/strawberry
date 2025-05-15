using OpenTK.Graphics.OpenGL;
using Strawberry.Graphics;
using Strawberry.Math;

namespace Strawberry.Desktop.Graphics
{
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

        PixelFormat format;

        public Texture(GraphicsContext gc, int width, int height, Color[] data, TextureSettings settings)
        {
            graphicsContext = gc;

            texture = GL.GenTexture();

            TextureWrapMode wrapS = TextureWrapMode.ClampToEdge;
            TextureWrapMode wrapT = TextureWrapMode.ClampToEdge;
            switch (settings.WrapS)
            {
                case TextureWrap.Repeat:
                    wrapS = TextureWrapMode.Repeat;
                    break;
                case TextureWrap.ClampToEdge:
                    wrapS = TextureWrapMode.ClampToEdge;
                    break;
                case TextureWrap.MirroredRepeat:
                    wrapS = TextureWrapMode.MirroredRepeat;
                    break;
            }

            switch (settings.WrapT)
            {
                case TextureWrap.Repeat:
                    wrapT = TextureWrapMode.Repeat;
                    break;
                case TextureWrap.ClampToEdge:
                    wrapT = TextureWrapMode.ClampToEdge;
                    break;
                case TextureWrap.MirroredRepeat:
                    wrapT = TextureWrapMode.MirroredRepeat;
                    break;
            }

            TextureMinFilter minFilter = TextureMinFilter.Nearest;
            TextureMagFilter magFilter = TextureMagFilter.Nearest;

            switch (settings.MinFilter)
            {
                case TextureFiltering.Linear:
                    minFilter = TextureMinFilter.Linear;
                    break;
                case TextureFiltering.Nearest:
                    minFilter = TextureMinFilter.Nearest;
                    break;
            }
            switch (settings.MagFilter)
            {
                case TextureFiltering.Linear:
                    magFilter = TextureMagFilter.Linear;
                    break;
                case TextureFiltering.Nearest:
                    magFilter = TextureMagFilter.Nearest;
                    break;
            }

            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)wrapS);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)wrapT);
            PixelInternalFormat internalFormat = PixelInternalFormat.Rgba;
            PixelFormat pformat = PixelFormat.Rgba;
            switch (settings.Format)
            {
                case TextureFormat.R8G8B8A8:
                    pformat = PixelFormat.Rgba;
                    internalFormat = PixelInternalFormat.Rgba;
                    break;
                case TextureFormat.B8G8R8A8:
                    pformat = PixelFormat.Bgra;
                    internalFormat = PixelInternalFormat.Rgba;
                    break;
                case TextureFormat.A:
                    pformat = PixelFormat.Alpha;
                    internalFormat = PixelInternalFormat.Alpha8;
                    break;
            }

            GL.TexImage2D<Color>(TextureTarget.Texture2D, 0, internalFormat,
                width, height, 0, pformat, PixelType.Float, data);
            format = pformat;
            Width = width;
            Height = height;

            TextureSettings = settings;
        }

        public Texture(GraphicsContext gc, int width, int height, byte[] data, TextureSettings settings)
        {
            graphicsContext = gc;

            texture = GL.GenTexture();
            TextureWrapMode wrapS = TextureWrapMode.ClampToEdge;
            TextureWrapMode wrapT = TextureWrapMode.ClampToEdge;
            switch (settings.WrapS)
            {
                case TextureWrap.Repeat:
                    wrapS = TextureWrapMode.Repeat;
                    break;
                case TextureWrap.ClampToEdge:
                    wrapS = TextureWrapMode.ClampToEdge;
                    break;
                case TextureWrap.MirroredRepeat:
                    wrapS = TextureWrapMode.MirroredRepeat;
                    break;
            }

            switch (settings.WrapT)
            {
                case TextureWrap.Repeat:
                    wrapT = TextureWrapMode.Repeat;
                    break;
                case TextureWrap.ClampToEdge:
                    wrapT = TextureWrapMode.ClampToEdge;
                    break;
                case TextureWrap.MirroredRepeat:
                    wrapT = TextureWrapMode.MirroredRepeat;
                    break;
            }

            TextureMinFilter minFilter = TextureMinFilter.Nearest;
            TextureMagFilter magFilter = TextureMagFilter.Nearest;

            switch (settings.MinFilter)
            {
                case TextureFiltering.Linear:
                    minFilter = TextureMinFilter.Linear;
                    break;
                case TextureFiltering.Nearest:
                    minFilter = TextureMinFilter.Nearest;
                    break;
            }
            switch (settings.MagFilter)
            {
                case TextureFiltering.Linear:
                    magFilter = TextureMagFilter.Linear;
                    break;
                case TextureFiltering.Nearest:
                    magFilter = TextureMagFilter.Nearest;
                    break;
            }

            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)wrapS);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)wrapT);
            PixelInternalFormat internalFormat = PixelInternalFormat.Rgba;
            PixelFormat pformat = PixelFormat.Rgba;
            switch (settings.Format)
            {
                case TextureFormat.R8G8B8A8:
                    pformat = PixelFormat.Rgba;
                    internalFormat = PixelInternalFormat.Rgba;
                    break;
                case TextureFormat.B8G8R8A8:
                    pformat = PixelFormat.Bgra;
                    internalFormat = PixelInternalFormat.Rgba;
                    break;
                case TextureFormat.A:
                    pformat = PixelFormat.Alpha;
                    internalFormat = PixelInternalFormat.Alpha8;
                    break;
            }


            GL.TexImage2D<byte>(TextureTarget.Texture2D, 0, internalFormat,
                width, height, 0, pformat, PixelType.UnsignedByte, data);

            format = pformat;
            Width = width;
            Height = height;

            TextureSettings = settings;
        }

        public override void Activate(Strawberry.Graphics.Shader shader, string name)
        {
            Shader s = (Shader)shader;
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.Uniform1(GL.GetUniformLocation(s.Program, name), 0);
        }


        protected override void CleanManaged()
        {
            GL.DeleteTexture(texture);
        }


        public override void SetFilter(TextureFiltering minFilter, TextureFiltering magFilter)
        {
            TextureMinFilter min = TextureMinFilter.Linear;
            if (minFilter == TextureFiltering.Nearest)
                min = TextureMinFilter.Nearest;

            TextureMagFilter mag = TextureMagFilter.Linear;
            if (magFilter == TextureFiltering.Nearest)
                mag = TextureMagFilter.Nearest;

            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)min);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)mag);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public override byte[] CopyToByteArray()
        {
            throw new NotImplementedException();
        }

        public override void Update(byte[] data)
        {
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, Width, Height, format, PixelType.UnsignedByte, data);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
    }
}
