/*
 * Strawberry Game Engine
 * File: TextRendererComponent.cs
 * Author: Koosha Aabedini Nassab
 *
 * Component that integrates text rendering into a scene or layer.
 */

using Strawberry.Core;
using Strawberry.Graphics.Layers;
using Strawberry.Math;

namespace Strawberry.Graphics.Text;

/// <summary>
/// A component that renders text through a <see cref="SpriteLayer"/>.
/// </summary>
public class TextRendererComponent : BaseComponent
{
    /// <summary>
    /// Gets or sets the sprite layer used for rendering the text.
    /// </summary>
    public SpriteLayer Layer { get; set; }

    /// <summary>
    /// Gets or sets the font used to draw the text.
    /// </summary>
    public Font Font { get; set; }

    /// <summary>
    /// Gets or sets the position of the rendered text.
    /// </summary>
    public Vector2 Position { get; set; }

    /// <summary>
    /// Gets or sets the text string to render.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Gets or sets the text color.
    /// </summary>
    public Color Color { get; set; } = Color.Black;

    /// <summary>
    /// Gets or sets the font size used for rendering.
    /// </summary>
    public float Size { get; set; } = 12.0f;

    /// <summary>
    /// Gets or sets the alignment of the text.
    /// </summary>
    public TextAlign TextAlign { get; set; } = TextAlign.Right;

    /// <summary>
    /// Gets or sets the direction in which text is rendered. Default is right-to-left.
    /// </summary>
    public TextDirection TextDirection { get; set; } = TextDirection.RightToLeft;

    /// <summary>
    /// Called when the component should render its content.
    /// </summary>
    public override void OnRender()
    {
        base.OnRender();
        if (Font != null && !string.IsNullOrEmpty(Text) && Layer != null)
        {
            TextRenderer.Draw(Layer, Font, Text, Position, Color, TextAlign, TextDirection, false, Size);
        }
    }
}
