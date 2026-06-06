/*
 * Strawberry Game Engine
 * File: Enums.cs
 * Author: Koosha Aabedini Nassab
 *
 * Graphics-related enumerations for textures, wrapping, and shaders.
 */

namespace Strawberry.Graphics
{
    /// <summary>
    /// The texture filtering modes.
    /// </summary>
    public enum TextureFiltering
    {
        /// <summary>
        /// Nearest neighbor filtering. identifies the single closest pixel (or "texel") to the coordinate center and uses its exact color
        /// </summary>
        Nearest,
        /// <summary>
        /// Linear filtering. Interpolates vertically and horizontally between the four pixels surrounding a coordinate.
        /// </summary>
        Linear
    }

    /// <summary>
    /// The texture formats.
    /// </summary>
    public enum TextureFormat
    {
        /// <summary>
        /// RGBA 8-bit per channel. (Red 8, Green 8, Blue 8, Alpha 8)
        /// </summary>
        R8G8B8A8,
        /// <summary>
        /// 8-bit Alpha.
        /// </summary>
        A,
        /// <summary>
        /// BGRA 8-bit per channel. (Blue 8, Green 8, Red 8, Alpha 8)
        /// </summary>
        B8G8R8A8
    }
    /// <summary>
    /// The texture wrapping modes.
    /// </summary>
    public enum TextureWrap
    {
        /// <summary>
        /// Clamps the texture coordinates to the range [0.0f, 1.0f].
        /// </summary>
        ClampToEdge,
        /// <summary>
        /// Repeats the texture coordinates (for values outside of the range [0.0f, 1.0f]).
        /// </summary>
        Repeat,
        /// <summary>
        /// Repeats the texture coordinates mirrored (for values outside of the range [0.0f, 1.0f]).
        /// </summary>
        MirroredRepeat
    }

    /// <summary>
    /// The element formats used by shaders
    /// </summary>
    public enum ElementFormats
    {
        /// <summary>
        /// 2-dimensional vector
        /// </summary>
        Position2,
        /// <summary>
        /// A float4 color.
        /// </summary>
        Color,
    }

    /// <summary>
    /// The type of geometry to use for rendering.
    /// </summary>
    public enum GeometryType
    {
        /// <summary>
        /// Static geometry that does not change.
        /// </summary>
        Static,
        /// <summary>
        /// Dynamic geometry that can be updated during runtime.
        /// </summary>
        Dynamic
    }

    /// <summary>
    /// The type of shader to use. HLSL is not supported in the current version. it may soon be removed.
    /// </summary>
    public enum ShaderLanguage
    {
        /// <summary>
        /// High Level Shading Language
        /// </summary>
        HLSL,
        /// <summary>
        /// OpenGL Shading Language
        /// </summary>
        GLSL
    }
}
