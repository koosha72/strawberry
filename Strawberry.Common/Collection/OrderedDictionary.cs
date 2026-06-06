/*
 * Strawberry Game Engine
 * File: OrderedDictionary.cs
 * Author: Koosha Aabedini Nassab
 *
 * Generic ordered dictionary wrapper around System.Collections.Specialized.OrderedDictionary.
 */

using System.Collections;
using System.Collections.Specialized;

namespace Strawberry.Collection
{
    /// <summary>
    /// Represents an Ordered Collection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="K"></typeparam>
    public class OrderedDictionary<T, K>
    {
        public OrderedDictionary UnderlyingCollection { get; } = new OrderedDictionary();

        public K this[T key]
        {
            get
            {
                return (K)UnderlyingCollection[key];
            }
            set
            {
                UnderlyingCollection[key] = value;
            }
        }

        public K this[int index]
        {
            get
            {
                return (K)UnderlyingCollection[index];
            }
            set
            {
                UnderlyingCollection[index] = value;
            }
        }
        public ICollection<T> Keys => UnderlyingCollection.Keys.OfType<T>().ToList();
        public ICollection<K> Values => UnderlyingCollection.Values.OfType<K>().ToList();
        public bool IsReadOnly => UnderlyingCollection.IsReadOnly;
        public int Count => UnderlyingCollection.Count;
        public IDictionaryEnumerator GetEnumerator() => UnderlyingCollection.GetEnumerator();
        public void Insert(int index, T key, K value) => UnderlyingCollection.Insert(index, key, value);
        public void RemoveAt(int index) => UnderlyingCollection.RemoveAt(index);
        public bool Contains(T key) => UnderlyingCollection.Contains(key);
        public void Add(T key, K value) => UnderlyingCollection.Add(key, value);
        public void Clear() => UnderlyingCollection.Clear();
        public void Remove(T key) => UnderlyingCollection.Remove(key);
        public void CopyTo(Array array, int index) => UnderlyingCollection.CopyTo(array, index);

        public void Swap(T key1, T key2)
        {
            K temp = this[key1];
            K temp2 = this[key2];
            int i = 0;
            for (i = 0; i < UnderlyingCollection.Count; i++)
            {
                if (((K)UnderlyingCollection[i]).Equals(temp))
                    break;
            }

            int j = 0;
            for (j = 0; j < UnderlyingCollection.Count; j++)
            {
                if (((K)UnderlyingCollection[j]).Equals(temp2))
                    break;
            }

            UnderlyingCollection.Remove(key1);
            UnderlyingCollection.Remove(key2);
            Insert(i, key2, temp2);
            Insert(j, key1, temp);
        }
    }
}
