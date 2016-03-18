using System;
using System.Collections;
using System.Collections.Generic;

namespace MyDictionary
{
    public class DictionaryImplementedWithList<TKey, TValue> : IDictionary<TKey, TValue>
    {
        internal class CaseInsensitiveStringComparer : IEqualityComparer<TKey>
        {
            public bool Equals(TKey x, TKey y)
            {
                if (typeof(TKey) == typeof(string))
                {
                    var stringX = (string)Convert.ChangeType(x, typeof(string));
                    var stringY = (string)Convert.ChangeType(y, typeof(string));

                    return (stringX.Equals(stringY, StringComparison.OrdinalIgnoreCase));
                }
                return x.Equals(y);
            }

            public int GetHashCode(TKey obj)
            {
                return obj.GetHashCode();
            }
        }
        private readonly List<TKey> internalKeys;
        private readonly List<TValue> internalValues;
        private int count;
        private readonly IEqualityComparer<TKey> keyComparer;

        public DictionaryImplementedWithList()
        {
            this.internalKeys = new List<TKey>();
            this.internalValues = new List<TValue>();
            this.count = 0;
            if (typeof(TKey) == typeof(string))
            {
                this.keyComparer = new CaseInsensitiveStringComparer();
            }
        }


        public DictionaryImplementedWithList(IEqualityComparer<TKey> keyComparer):
            this()
        {
            this.keyComparer = keyComparer;
        }

        public TValue this[TKey key]
        {
            get
            {
                if (!this.ContainsKey(key))
                {
                    throw new KeyNotFoundException();
                }
                return this.internalValues[GetIndexOfKey(key)];
            }

            set
            {
                if (!this.ContainsKey(key))
                {
                    throw new KeyNotFoundException();
                }
                this.internalValues[GetIndexOfKey(key)] = value;
            }
        }

        public int Count
        {
            get
            {
                return this.count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                return this.internalKeys;
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                return this.internalValues;
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Add(TKey key, TValue value)
        {
            if (ContainsKey(key))
            {
                throw new ArgumentException("Key already exists");
            }
            this.internalKeys.Add(key);
            this.internalValues.Add(value);
            this.count++;
        }

        public void Clear()
        {
            this.internalKeys.Clear();
            this.internalValues.Clear();
            this.count = 0;
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return
                ContainsKey(item.Key)
                && this.internalValues.Contains(item.Value);
        }

        public bool ContainsKey(TKey key)
        {
            if (this.keyComparer != null)
            {
                return this.internalKeys.Exists(k => this.keyComparer.Equals(key, k));
            }
            return this.internalKeys.Contains(key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            // TODO :: Validate params and throw if needed
            int counter = 0;
            while (counter < this.count)
            {
                array[arrayIndex + counter] = new KeyValuePair<TKey, TValue>(this.internalKeys[counter], this.internalValues[counter]);
                counter++;
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            int counter = 0;
            while (counter < this.count)
            {
                var kvp = new KeyValuePair<TKey, TValue>(this.internalKeys[counter], this.internalValues[counter]);
                counter++;
                yield return kvp;
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (!Contains(item))
            {
                return false;
            }
            return Remove(item.Key);
        }

        public bool Remove(TKey key)
        {
            if (!ContainsKey(key))
            {
                return false;
            }
            this.internalValues.RemoveAt(GetIndexOfKey(key));
            this.internalKeys.Remove(key);
            this.count--;

            return true;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (!ContainsKey(key))
            {
                value = default(TValue);
                return false;
            }

            value = this[key];
            return true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private int GetIndexOfKey(TKey key)
        {
            if (this.keyComparer != null)
            {
                return this.internalKeys.FindIndex(k => this.keyComparer.Equals(key, k));
            }
            return this.internalKeys.IndexOf(key);
        }
    }
}
