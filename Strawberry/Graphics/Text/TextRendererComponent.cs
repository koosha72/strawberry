/*
 * Strawberry Game Engine
 * File: TextRendererComponent.cs
 * Author: Koosha Aabedini Nassab
 *
 * Component that integrates text rendering into a scene or layer.
 */

using Strawberry.Components;
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
    /// Gets or sets the alignment of the text. Default is left.
    /// </summary>
    public TextAlign TextAlign { get; set; } = TextAlign.Left;

    /// <summary>
    /// Gets or sets the direction in which text is rendered. Default is left-to-right.
    /// </summary>
    public TextDirection TextDirection { get; set; } = TextDirection.LeftToRight;

    /// <summary>
    /// Whether the numbers should be forced to rendered as persian, Default is false
    /// </summary>
    public bool ForcePersianDigits { get; set; } = false;

    /// <summary>
    /// Whether the text should be visible or not. Default is true
    /// </summary>
    public bool Visible { get; set; } = true;

    TransformComponent transform;

    public override void OnComponentAdded(BaseComponent component)
    {
        base.OnComponentAdded(component);
        if (component is TransformComponent && transform == null)
            transform = component as TransformComponent;
    }

    public override void OnRender()
    {
        base.OnRender();
        if (Visible && Font != null && !string.IsNullOrEmpty(Text) && Layer != null)
        {
            TextRenderer.Draw(Layer, Font, Text, transform.Position, Color, TextAlign, TextDirection, ForcePersianDigits, Size);
        }
    }
}
