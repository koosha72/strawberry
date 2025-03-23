namespace Strawberry.Graphics.Layers
{
    public class IsometricComparer : IComparer<SpriteQuad>
    {
        public int Compare(SpriteQuad a, SpriteQuad b)
        {
            return (int)(a.XYUV1.Y - b.XYUV1.Y);
        }
    }

    public class IsometricRenderingSorter : IRenderingSorter
    {
        public void Sort(List<SpriteQuad> quads)
        {
            quads.Sort(new IsometricComparer());
        }
    }
}
