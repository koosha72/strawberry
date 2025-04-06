using System;

namespace Strawberry.Graphics;

public struct TextureSettings
{
    public TextureFormat Format;
    public TextureFiltering MinFilter;
    public TextureFiltering MagFilter;
    public TextureWrap WrapS;
    public TextureWrap WrapT;

    public TextureSettings()
    {
        Format = TextureFormat.R8G8B8A8;
        MinFilter = TextureFiltering.Nearest;
        MagFilter = TextureFiltering.Nearest;
        WrapS = TextureWrap.ClampToEdge;
        WrapT = TextureWrap.ClampToEdge;
    }
}
