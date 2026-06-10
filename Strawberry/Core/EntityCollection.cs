/*
 * Strawberry Game Engine
 * File: EntityCollection.cs
 * Author: Koosha Aabedini Nassab
 *
 * Strongly-typed dictionary collection for game entities.
 */

using System.Collections;

namespace Strawberry.Core
{
    /// <summary>
    /// Represents a strongly-typed collection of <see cref="Entity"/> objects, accessible by their string keys.
    /// Wraps a dictionary and implements both mutable and read-only dictionary interfaces.
    /// </summary>
    public class EntityCollection : IDictionary<string, Entity>,
                IReadOnlyDictionary<string, Entity>
    {
        List<Entity> data = new List<Entity>();
        Dictionary<string, int> indexMap = new Dictionary<string, int>();

        List<(string key, Entity entity)> pendingAdds = new();
        HashSet<string> pendingRemoves = new();

        bool pendingClear = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityCollection"/> class.
        /// </summary>
        public EntityCollection()
        {
        }

        /// <summary>
        /// Gets or sets the <see cref="Entity"/> with the specified key. 
        /// Returns <c>null</c> if the key is not found when getting.
        /// </summary>
        /// <param name="key">The string key of the entity to get or set.</param>
        /// <returns>The entity associated with the specified key, or <c>null</c> if the key does not exist.</returns>
        public Entity this[string key]
        {
            get
            {
                return indexMap.TryGetValue(key, out int index) ? data[index] : null;
            }
            set
            {
                if (indexMap.TryGetValue(key, out int index))
                {
                    data[index] = value;
                }
                else
                {
                    pendingAdds.Add((key, value));
                }
            }
        }

        /// <summary>
        /// Gets the number of entities contained in the collection.
        /// </summary>
        public int Count => data.Count;

        /// <summary>
        /// Gets a value indicating whether the <see cref="IDictionary"/> object is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets an enumerable collection that contains the keys in the read-only dictionary.
        /// </summary>
        IEnumerable<string> IReadOnlyDictionary<string, Entity>.Keys
        {
            get
            {
                return indexMap.Keys;
            }
        }

        /// <summary>
        /// Gets an <see cref="ICollection{String}"/> containing the keys of the dictionary.
        /// </summary>
        ICollection<string> IDictionary<string, Entity>.Keys
        {
            get
            {
                return indexMap.Keys;
            }
        }

        /// <summary>
        /// Gets an enumerable collection that contains the values in the read-only dictionary.
        /// </summary>
        IEnumerable<Entity> IReadOnlyDictionary<string, Entity>.Values
        {
            get
            {
                return data;
            }
        }

        /// <summary>
        /// Gets an <see cref="ICollection{Entity}"/> containing the values in the dictionary.
        /// </summary>
        ICollection<Entity> IDictionary<string, Entity>.Values
        {
            get
            {
                return data;
            }
        }

        /// <summary>
        /// Gets a list containing all the entities in the collection. 
        /// Note: This creates a new list allocation each time it is accessed.
        /// </summary>
        public IList<Entity> Values
        {
            get { return data; }
        }

        /// <summary>
        /// Gets a list containing all the keys in the collection. 
        /// Note: This creates a new list allocation each time it is accessed.
        /// </summary>
        public IList<string> Keys
        {
            get { return indexMap.Keys.ToList(); }
        }

        /// <summary>
        /// Adds an item to the <see cref="ICollection{KeyValuePair}"/>.
        /// </summary>
        /// <param name="item">The key-value pair to add.</param>
        void ICollection<KeyValuePair<string, Entity>>.Add(KeyValuePair<string, Entity> item)
        {
            Add(item.Key, item.Value);
        }

        /// <summary>
        /// Adds an entity with the specified key to the collection.  The changes to the collection will not apply until <see cref="Flush"/> is called.
        /// </summary>
        /// <param name="key">The key of the entity to add.</param>
        /// <param name="value">The entity to add.</param>
        public void Add(string key, Entity value)
        {
            pendingAdds.Add((key, value));
        }

        private void ImmediateAdd(string key, Entity value)
        {
            indexMap.Add(key, data.Count);
            data.Add(value);
        }

        /// <summary>
        /// Removes all entities from the collection.
        /// </summary>
        public void Clear()
        {
            pendingClear = true;
            pendingAdds.Clear();
            pendingRemoves.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="ICollection{KeyValuePair}"/> contains a specific key-value pair.
        /// </summary>
        /// <param name="item">The key-value pair to locate in the collection.</param>
        /// <returns><c>true</c> if the item is found; otherwise, <c>false</c>.</returns>
        bool ICollection<KeyValuePair<string, Entity>>.Contains(KeyValuePair<string, Entity> item)
        {
            return indexMap.ContainsKey(item.Key) && data.Contains(item.Value);
        }

        /// <summary>
        /// Determines whether the collection contains an entity with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the collection.</param>
        /// <returns><c>true</c> if the collection contains an entity with the key; otherwise, <c>false</c>.</returns>
        public bool ContainsKey(string key)
        {
            return indexMap.ContainsKey(key);
        }

        /// <summary>
        /// Copies the elements of the <see cref="ICollection{KeyValuePair}"/> to an array, starting at the specified array index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the collection.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        void ICollection<KeyValuePair<string, Entity>>.CopyTo(KeyValuePair<string, Entity>[] array, int arrayIndex)
        {
            foreach (var kvp in indexMap)
            {
                array[arrayIndex++] = new KeyValuePair<string, Entity>(kvp.Key, data[kvp.Value]);
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="ICollection{KeyValuePair}"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="ICollection{KeyValuePair}"/>.</param>
        /// <returns><c>true</c> if the item was successfully removed; otherwise, <c>false</c>.</returns>
        bool ICollection<KeyValuePair<string, Entity>>.Remove(KeyValuePair<string, Entity> item)
        {
            var res = Remove(item.Key);

            return res;
        }

        /// <summary>
        /// Removes the entity with the specified key from the collection. The changes to the collection will not apply until <see cref="Flush"/> is called.
        /// </summary>
        /// <param name="key">The key of the entity to remove.</param>
        /// <returns><c>true</c> if the entity is successfully removed; otherwise, <c>false</c>.</returns>
        public bool Remove(string key)
        {
            for (int i = 0; i < pendingAdds.Count; i++)
            {
                if (pendingAdds[i].key == key)
                {
                    pendingAdds.RemoveAt(i);
                    return true;
                }
            }

            if (!indexMap.TryGetValue(key, out int index))
                return false;

            pendingRemoves.Add(key);
            return true;
        }

        public bool ImmediateRemove(string key)
        {
            if (!indexMap.TryGetValue(key, out int index))
                return false;

            int lastIndex = data.Count - 1;

            if (index != lastIndex)
            {
                // Swap with last element
                Entity lastEntity = data[lastIndex];
                data[index] = lastEntity;

                // Update the swapped entity's index in the map
                indexMap[lastEntity.ID] = index;
            }

            data.RemoveAt(lastIndex);  // O(1) — removing from end
            indexMap.Remove(key);
            return true;
        }

        /// <summary>
        /// Gets the entity associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the entity to get.</param>
        /// <param name="value">When this method returns, contains the entity associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter.</param>
        /// <returns><c>true</c> if the collection contains an entity with the specified key; otherwise, <c>false</c>.</returns>
        public bool TryGetValue(string key, out Entity value)
        {
            if (indexMap.TryGetValue(key, out int index))
            {
                value = data[index];
                return true;
            }
            value = null;
            return false;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator<KeyValuePair<string, Entity>> IEnumerable<KeyValuePair<string, Entity>>.GetEnumerator() => GetEnumerator();

        public IEnumerator<KeyValuePair<string, Entity>> GetEnumerator()
        {
            foreach (var kvp in indexMap)
                yield return new KeyValuePair<string, Entity>(kvp.Key, data[kvp.Value]);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Changes the key associated with an existing entity in the collection.
        /// This also updates the entity's <see cref="Entity.ID"/> property to match the new key.
        /// </summary>
        /// <param name="oldKey">The current key of the entity.</param>
        /// <param name="newKey">The new key to assign to the entity.</param>
        public void ChangeKey(string oldKey, string newKey)
        {
            int index = indexMap[oldKey];
            indexMap.Remove(oldKey);
            indexMap[newKey] = index;
            data[index].ID = newKey;
        }

        /// <summary>
        /// Applies all the pending changes to the collection.
        /// </summary>
        public void Flush()
        {

            if (pendingClear)
            {
                data.Clear();
                indexMap.Clear();
                pendingClear = false;
            }
            else
            {
                foreach (var key in pendingRemoves)
                    ImmediateRemove(key);
                pendingRemoves.Clear();
            }

            // There maybe new adds after the Clear call
            foreach (var (key, entity) in pendingAdds)
                ImmediateAdd(key, entity);
            pendingAdds.Clear();
        }
    }
}