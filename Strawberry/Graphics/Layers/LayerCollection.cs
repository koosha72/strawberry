using Strawberry.Collection;

namespace Strawberry.Graphics.Layers
{
    public class LayerCollection : OrderedDictionary<string, Layer>
    {
        public void MoveTo(int source, int target)
        {
            var key = Keys.ElementAt(source);
            var layer = this[source];
            RemoveAt(source);
            Insert(target, key, layer);
        }

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
