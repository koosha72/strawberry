/*
 * Strawberry Game Engine
 * File: IsometricRenderingSorter.cs
 * Author: Koosha Aabedini Nassab
 *
 * Isometric rendering sorter used to order sprite quads by vertical position.
 */

namespace Strawberry.Graphics.Layers
{
    /// <summary>
    /// Compares two sprite quads by vertical position
    /// </summary>
    public class IsometricComparer : IComparer<SpriteQuad>
    {
        public int Compare(SpriteQuad a, SpriteQuad b)
        {
            return (int)(a.XYUV1.Y - b.XYUV1.Y);
        }
    }

    /// <summary>
    /// Sorts sprite quads by vertical position for isometric rendering
    /// </summary>
    public class IsometricRenderingSorter : IRenderingSorter
    {
        public void Sort(List<SpriteQuad> quads)
        {
            quads.Sort(new IsometricComparer());
        }
    }
}
