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
    public class EntityCollection : IDictionary, IDictionary<string, Entity>,
                IReadOnlyDictionary<string, Entity>
    {
        /// <summary>
        /// The underlying dictionary that stores the entities.
        /// </summary>
        Dictionary<string, Entity> list;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityCollection"/> class.
        /// </summary>
        public EntityCollection()
        {
            list = new Dictionary<string, Entity>();
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
                if (list.ContainsKey(key))
                    return list[key];
                else
                    return null;
            }

            set
            {
                list[key] = value;
            }
        }

        /// <summary>
        /// Gets or sets the element with the specified object key.
        /// </summary>
        /// <param name="key">The object key of the element to get or set.</param>
        /// <returns>The element with the specified key.</returns>
        object IDictionary.this[object key]
        {
            get
            {
                return (list as IDictionary)[key];
            }

            set
            {
                (list as IDictionary)[key] = value;
            }
        }

        /// <summary>
        /// Gets the number of entities contained in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                return list.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="IDictionary"/> object has a fixed size.
        /// </summary>
        public bool IsFixedSize
        {
            get
            {
                return (list as IDictionary).IsFixedSize;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="IDictionary"/> object is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return (list as IDictionary).IsReadOnly;
            }
        }

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="ICollection"/> is synchronized (thread safe).
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                return (list as IDictionary).IsSynchronized;
            }
        }

        /// <summary>
        /// Gets an <see cref="ICollection"/> object containing the keys of the <see cref="IDictionary"/> object.
        /// </summary>
        ICollection IDictionary.Keys
        {
            get
            {
                return (list as IDictionary).Keys;
            }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="ICollection"/>.
        /// </summary>
        object ICollection.SyncRoot
        {
            get
            {
                return (list as IDictionary).SyncRoot;
            }
        }

        /// <summary>
        /// Gets an <see cref="ICollection"/> object containing the values in the <see cref="IDictionary"/> object.
        /// </summary>
        ICollection IDictionary.Values
        {
            get
            {
                return (list as IDictionary).Values;
            }
        }

        /// <summary>
        /// Gets an enumerable collection that contains the keys in the read-only dictionary.
        /// </summary>
        IEnumerable<string> IReadOnlyDictionary<string, Entity>.Keys
        {
            get
            {
                return (list as IReadOnlyDictionary<string, Entity>).Keys;
            }
        }

        /// <summary>
        /// Gets an <see cref="ICollection{String}"/> containing the keys of the dictionary.
        /// </summary>
        ICollection<string> IDictionary<string, Entity>.Keys
        {
            get
            {
                return list.Keys;
            }
        }

        /// <summary>
        /// Gets an enumerable collection that contains the values in the read-only dictionary.
        /// </summary>
        IEnumerable<Entity> IReadOnlyDictionary<string, Entity>.Values
        {
            get
            {
                return (list as IReadOnlyDictionary<string, Entity>).Values;
            }
        }

        /// <summary>
        /// Gets an <see cref="ICollection{Entity}"/> containing the values in the dictionary.
        /// </summary>
        ICollection<Entity> IDictionary<string, Entity>.Values
        {
            get
            {
                return list.Values;
            }
        }

        /// <summary>
        /// Gets a list containing all the entities in the collection. 
        /// Note: This creates a new list allocation each time it is accessed.
        /// </summary>
        public IList<Entity> Values
        {
            get { return list.Values.ToList(); }
        }

        /// <summary>
        /// Gets a list containing all the keys in the collection. 
        /// Note: This creates a new list allocation each time it is accessed.
        /// </summary>
        public IList<string> Keys
        {
            get { return list.Keys.ToList(); }
        }

        /// <summary>
        /// Adds an item to the <see cref="ICollection{KeyValuePair}"/>.
        /// </summary>
        /// <param name="item">The key-value pair to add.</param>
        void ICollection<KeyValuePair<string, Entity>>.Add(KeyValuePair<string, Entity> item)
        {
            list.Add(item.Key, item.Value);
        }

        /// <summary>
        /// Adds an entity with the specified key to the collection.
        /// </summary>
        /// <param name="key">The key of the entity to add.</param>
        /// <param name="value">The entity to add.</param>
        public void Add(string key, Entity value)
        {
            list.Add(key, value);
        }

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="IDictionary"/> object.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        void IDictionary.Add(object key, object value)
        {
            (list as IDictionary).Add(key, value);
        }

        /// <summary>
        /// Removes all entities from the collection.
        /// </summary>
        public void Clear()
        {
            list.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="ICollection{KeyValuePair}"/> contains a specific key-value pair.
        /// </summary>
        /// <param name="item">The key-value pair to locate in the collection.</param>
        /// <returns><c>true</c> if the item is found; otherwise, <c>false</c>.</returns>
        bool ICollection<KeyValuePair<string, Entity>>.Contains(KeyValuePair<string, Entity> item)
        {
            return (list as IDictionary<string, Entity>).Contains(item);
        }

        /// <summary>
        /// Determines whether the <see cref="IDictionary"/> object contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="IDictionary"/> object.</param>
        /// <returns><c>true</c> if the <see cref="IDictionary"/> contains an element with the key; otherwise, <c>false</c>.</returns>
        bool IDictionary.Contains(object key)
        {
            return (list as IDictionary).Contains(key);
        }

        /// <summary>
        /// Determines whether the collection contains an entity with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the collection.</param>
        /// <returns><c>true</c> if the collection contains an entity with the key; otherwise, <c>false</c>.</returns>
        public bool ContainsKey(string key)
        {
            return list.ContainsKey(key);
        }

        /// <summary>
        /// Copies the elements of the <see cref="ICollection{KeyValuePair}"/> to an array, starting at the specified array index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the collection.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        void ICollection<KeyValuePair<string, Entity>>.CopyTo(KeyValuePair<string, Entity>[] array, int arrayIndex)
        {
            (list as IDictionary<string, Entity>).CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Copies the elements of the <see cref="ICollection"/> to an array, starting at the specified array index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the collection.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        void ICollection.CopyTo(Array array, int index)
        {
            (list as IDictionary).CopyTo(array, index);
        }

        /// <summary>
        /// Returns an <see cref="IDictionaryEnumerator"/> for the <see cref="IDictionary"/>.
        /// </summary>
        /// <returns>An <see cref="IDictionaryEnumerator"/> for the <see cref="IDictionary"/>.</returns>
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return (list as IDictionary).GetEnumerator();
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="ICollection{KeyValuePair}"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="ICollection{KeyValuePair}"/>.</param>
        /// <returns><c>true</c> if the item was successfully removed; otherwise, <c>false</c>.</returns>
        bool ICollection<KeyValuePair<string, Entity>>.Remove(KeyValuePair<string, Entity> item)
        {
            var res = (list as IDictionary<string, Entity>).Remove(item);

            return res;
        }

        /// <summary>
        /// Removes the entity with the specified key from the collection.
        /// </summary>
        /// <param name="key">The key of the entity to remove.</param>
        /// <returns><c>true</c> if the entity is successfully removed; otherwise, <c>false</c>.</returns>
        public bool Remove(string key)
        {
            var res = list.Remove(key);

            return res;
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="IDictionary"/> object.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        void IDictionary.Remove(object key)
        {
            (list as IDictionary).Remove(key);
        }

        /// <summary>
        /// Gets the entity associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the entity to get.</param>
        /// <param name="value">When this method returns, contains the entity associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter.</param>
        /// <returns><c>true</c> if the collection contains an entity with the specified key; otherwise, <c>false</c>.</returns>
        public bool TryGetValue(string key, out Entity value)
        {
            return list.TryGetValue(key, out value);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator<KeyValuePair<string, Entity>> IEnumerable<KeyValuePair<string, Entity>>.GetEnumerator()
        {
            return (list as IEnumerable<KeyValuePair<string, Entity>>).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (list as IEnumerable).GetEnumerator();
        }

        /// <summary>
        /// Changes the key associated with an existing entity in the collection.
        /// This also updates the entity's <see cref="Entity.ID"/> property to match the new key.
        /// </summary>
        /// <param name="oldKey">The current key of the entity.</param>
        /// <param name="newKey">The new key to assign to the entity.</param>
        public void ChangeKey(string oldKey, string newKey)
        {
            Entity val = list[oldKey];
            list.Remove(oldKey);
            list.Add(newKey, val);
            val.ID = newKey;
        }
    }
}