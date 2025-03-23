namespace Strawberry.Graphics.Layers
{
    public class IsometricRenderingSorter : IRenderingSorter
    {
        public void Sort(List<SpriteQuad> quads)
        {
            quads.Sort((a, b) => (int)(a.XYUV1.Y - b.XYUV1.Y));
        }
    }
}
