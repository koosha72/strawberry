using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strawberry.Core
{
    /// <summary>
    /// A collection of entities.
    /// </summary>
    public class EntityCollection : IDictionary, IDictionary<string, Entity>,
                IReadOnlyDictionary<string, Entity>
    {
        Dictionary<string, Entity> list;

        public EntityCollection()
        {
            list = new Dictionary<string, Entity>();
        }
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

        public int Count
        {
            get
            {
                return list.Count;
            }
        }

        public bool IsFixedSize
        {
            get
            {
                return (list as IDictionary).IsFixedSize;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return (list as IDictionary).IsReadOnly;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return (list as IDictionary).IsSynchronized;
            }
        }

        ICollection IDictionary.Keys
        {
            get
            {
                return (list as IDictionary).Keys;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return (list as IDictionary).SyncRoot;
            }
        }

        ICollection IDictionary.Values
        {
            get
            {
                return (list as IDictionary).Values;
            }
        }

        IEnumerable<string> IReadOnlyDictionary<string, Entity>.Keys
        {
            get
            {
                return (list as IReadOnlyDictionary<string, Entity>).Keys;
            }
        }

        ICollection<string> IDictionary<string, Entity>.Keys
        {
            get
            {
                return list.Keys;
            }
        }

        IEnumerable<Entity> IReadOnlyDictionary<string, Entity>.Values
        {
            get
            {
                return (list as IReadOnlyDictionary<string, Entity>).Values;
            }
        }

        ICollection<Entity> IDictionary<string, Entity>.Values
        {
            get
            {
                return list.Values;
            }
        }

        public IList<Entity> Values
        {
            get { return list.Values.ToList(); }
        }

        public IList<string> Keys
        {
            get { return list.Keys.ToList(); }
        }

        void ICollection<KeyValuePair<string, Entity>>.Add(KeyValuePair<string, Entity> item)
        {
            list.Add(item.Key, item.Value);
        }

        public void Add(string key, Entity value)
        {
            list.Add(key, value);
        }

        void IDictionary.Add(object key, object value)
        {
            (list as IDictionary).Add(key, value);
        }

        public void Clear()
        {
            list.Clear();
        }

        bool ICollection<KeyValuePair<string, Entity>>.Contains(KeyValuePair<string, Entity> item)
        {
            return (list as IDictionary<string, Entity>).Contains(item);
        }

        bool IDictionary.Contains(object key)
        {
            return (list as IDictionary).Contains(key);
        }

        public bool ContainsKey(string key)
        {
            return list.ContainsKey(key);
        }

        void ICollection<KeyValuePair<string, Entity>>.CopyTo(KeyValuePair<string, Entity>[] array, int arrayIndex)
        {
            (list as IDictionary<string, Entity>).CopyTo(array, arrayIndex);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            (list as IDictionary).CopyTo(array, index);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return (list as IDictionary).GetEnumerator();
        }

        bool ICollection<KeyValuePair<string, Entity>>.Remove(KeyValuePair<string, Entity> item)
        {
            var res = (list as IDictionary<string, Entity>).Remove(item);

            return res;
        }

        public bool Remove(string key)
        {
            var res = list.Remove(key);

            return res;
        }

        void IDictionary.Remove(object key)
        {
            (list as IDictionary).Remove(key);
        }

        public bool TryGetValue(string key, out Entity value)
        {
            return list.TryGetValue(key, out value);
        }

        IEnumerator<KeyValuePair<string, Entity>> IEnumerable<KeyValuePair<string, Entity>>.GetEnumerator()
        {
            return (list as IEnumerable<KeyValuePair<string, Entity>>).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (list as IEnumerable).GetEnumerator();
        }

        public void ChangeKey(string oldKey, string newKey)
        {
            Entity val = list[oldKey];
            list.Remove(oldKey);
            list.Add(newKey, val);
            val.ID = newKey;
        }
    }
}
