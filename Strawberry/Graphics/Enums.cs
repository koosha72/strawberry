namespace Strawberry.Graphics
{
    public enum TextureFiltering
    {
        Nearest,
        Linear
    }

    public enum TextureFormat
    {
        R8G8B8A8,
        A,
        B8G8R8A8
    }

    public enum TextureWrap
    {
        ClampToEdge,
        Repeat,
        MirroredRepeat
    }

    public enum ElementFormats
    {
        Position2,
        Color,
    }

    public enum GeometryType
    {
        Static,
        Dynamic
    }

    public enum ShaderLanguage
    {
        HLSL,
        GLSL
    }
}
