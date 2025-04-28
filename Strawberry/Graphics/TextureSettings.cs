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

    public override string ToString() =>
    $"Format: {Format}, MinFilter: {MinFilter}, MagFilter: {MagFilter}, WrapS: {WrapS}, WrapT: {WrapT}";

    public override bool Equals(object obj) => obj is TextureSettings other && this == other;
    public override int GetHashCode() => HashCode.Combine(Format, MinFilter, MagFilter, WrapS, WrapT);
    public static bool operator ==(TextureSettings a, TextureSettings b) =>
        a.Format == b.Format &&
        a.MinFilter == b.MinFilter &&
        a.MagFilter == b.MagFilter &&
        a.WrapS == b.WrapS &&
        a.WrapT == b.WrapT;
    public static bool operator !=(TextureSettings a, TextureSettings b) => !(a == b);
}
