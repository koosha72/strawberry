/*
 * Strawberry Game Engine
 * File: TextureSettings.cs
 * Author: Koosha Aabedini Nassab
 *
 * Lightweight settings for texture creation and sampling.
 */

namespace Strawberry.Graphics;

/// <summary>
/// Represents texture settings for creation and sampling.
/// </summary>
public struct TextureSettings
{
    /// <summary>
    /// The <see cref="TextureFormat"/> used for the texture.
    /// </summary>
    public TextureFormat Format;

    /// <summary>
    /// The minification <see cref="TextureFiltering"/> mode.
    /// </summary>
    public TextureFiltering MinFilter;

    /// <summary>
    /// The magnification <see cref="TextureFiltering"/> mode.
    /// </summary>
    public TextureFiltering MagFilter;

    /// <summary>
    /// The horizontal <see cref="TextureWrap"/> mode.
    /// </summary>
    public TextureWrap WrapS;

    /// <summary>
    /// The vertical <see cref="TextureWrap"/> mode.
    /// </summary>
    public TextureWrap WrapT;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureSettings"/> struct with default values.
    /// </summary>
    public TextureSettings()
    {
        Format = TextureFormat.R8G8B8A8;
        MinFilter = TextureFiltering.Nearest;
        MagFilter = TextureFiltering.Nearest;
        WrapS = TextureWrap.ClampToEdge;
        WrapT = TextureWrap.ClampToEdge;
    }

    /// <summary>
    /// Returns a string representation of the current <see cref="TextureSettings"/>.
    /// </summary>
    public override string ToString() =>
        $"Format: {Format}, MinFilter: {MinFilter}, MagFilter: {MagFilter}, WrapS: {WrapS}, WrapT: {WrapT}";

    /// <summary>
    /// Determines whether the specified object is equal to the current <see cref="TextureSettings"/>.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    public override bool Equals(object obj) => obj is TextureSettings other && this == other;

    /// <summary>
    /// Returns a hash code for the current <see cref="TextureSettings"/>.
    /// </summary>
    public override int GetHashCode() => HashCode.Combine(Format, MinFilter, MagFilter, WrapS, WrapT);

    /// <summary>
    /// Compares two <see cref="TextureSettings"/> values for equality.
    /// </summary>
    /// <param name="a">The first value to compare.</param>
    /// <param name="b">The second value to compare.</param>
    /// <returns><c>true</c> if the values are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(TextureSettings a, TextureSettings b) =>
        a.Format == b.Format &&
        a.MinFilter == b.MinFilter &&
        a.MagFilter == b.MagFilter &&
        a.WrapS == b.WrapS &&
        a.WrapT == b.WrapT;

    /// <summary>
    /// Compares two <see cref="TextureSettings"/> values for inequality.
    /// </summary>
    /// <param name="a">The first value to compare.</param>
    /// <param name="b">The second value to compare.</param>
    /// <returns><c>true</c> if the values are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(TextureSettings a, TextureSettings b) => !(a == b);
}
