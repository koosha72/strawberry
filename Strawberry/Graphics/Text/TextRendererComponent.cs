using Strawberry.Core;
using Strawberry.Graphics.Layers;
using Strawberry.Math;

namespace Strawberry.Graphics.Text;

public class TextRendererComponent : BaseComponent
{
    public SpriteLayer Layer { get; set; }
    public Font Font { get; set; }
    public Vector2 Position { get; set; }
    public string Text { get; set; }
    public Color Color { get; set; } = Color.Black;
    public float Size { get; set; } = 12.0f;
    public TextAlign TextAlign { get; set; } = TextAlign.Right;
    public TextDirection TextDirection { get; set; } = TextDirection.RightToLeft;

    public override void OnRender()
    {
        base.OnRender();
        if (Font != null && !string.IsNullOrEmpty(Text) && Layer != null)
        {
            TextRenderer.Draw(Layer, Font, Text, Position, Color, TextAlign, TextDirection, false, Size);
        }
    }
}
