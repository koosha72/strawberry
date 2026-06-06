/*
 * Strawberry Game Engine
 * File: LayerCollection.cs
 * Author: Koosha Aabedini Nassab
 *
 * Collection managing the set of layers within a scene.
 */

using Strawberry.Collection;

namespace Strawberry.Graphics.Layers
{
    /// <summary>
    /// Collection managing the set of layers within a scene.
    /// </summary>
    public class LayerCollection : OrderedDictionary<string, Layer>
    {
        /// <summary>
        /// Moves a layer to from source index to the specified position (target) in the collection of layer
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public void MoveTo(int source, int target)
        {
            var key = Keys.ElementAt(source);
            var layer = this[source];
            RemoveAt(source);
            Insert(target, key, layer);
        }

        /// <summary>
        /// Returns the index of the specified layer
        /// </summary>
        /// <param name="value">The layer to find</param>
        /// <returns>The index of the specified layer</returns>
        public int IndexOf(Layer value)
        {
            int index = -1;
            foreach (var v in Values)
            {
                index++;
                if (v.Equals(value))
                    return index;
            }

            return -1;
        }
    }
}
