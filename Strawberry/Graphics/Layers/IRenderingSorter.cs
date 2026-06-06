/*
 * Strawberry Game Engine
 * File: IRenderingSorter.cs
 * Author: Koosha Aabedini Nassab
 *
 * Interface for sorting renderable sprites before drawing.
 */

namespace Strawberry.Graphics.Layers
{
    /// <summary>
    /// Defines a sorter for ordering sprite quads before rendering.
    /// </summary>
    public interface IRenderingSorter
    {
        /// <summary>
        /// Sorts the provided list of sprite quads in place.
        /// </summary>
        /// <param name="quads">The list of sprite quads to sort.</param>
        void Sort(List<SpriteQuad> quads);
    }
}
