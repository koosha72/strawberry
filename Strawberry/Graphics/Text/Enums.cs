/*
 * Strawberry Game Engine
 * File: Enums.cs
 * Author: Koosha Aabedini Nassab
 *
 * Text-related enums such as direction and alignment.
 */

namespace Strawberry.Graphics.Text;

/// <summary>
/// The direction of the text
/// </summary>
public enum TextDirection
{
    /// <summary>
    /// Left to right text rendering
    /// </summary>
    LeftToRight,
    /// <summary>
    /// Right to left text rendering (for persian language)
    /// </summary>
    RightToLeft,
    /// <summary>
    /// Automatic direction calculation (not supported yet)
    /// </summary>
    None
}

/// <summary>
/// The alignment of the text
/// </summary>
public enum TextAlign
{
    /// <summary>
    /// Left aligned
    /// </summary>
    Left,
    /// <summary>
    /// Right aligned
    /// </summary>
    Right,
    /// <summary>
    /// Center aligned
    /// </summary>
    Center
}