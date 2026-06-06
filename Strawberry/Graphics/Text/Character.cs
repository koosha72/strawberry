/*
 * Strawberry Game Engine
 * File: Character.cs
 * Author: Koosha Aabedini Nassab
 *
 * Glyph metrics structure used by the Font and text renderer.
 */

namespace Strawberry.Graphics.Text;

/// <summary>
/// Glyph metrics structure used by the Font and text renderer.
/// </summary>
public struct Character
{
    public double Right;
    public double Bottom;
    public double Left;
    public double Top;
    public double Adwidth;
    public double Adheight;
}