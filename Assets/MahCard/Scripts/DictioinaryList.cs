using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace HK
{
    [Serializable]
    public abstract class DictionaryList<TKey, TValue>
    {
        [SerializeField]
        private List<TValue> list;

        private Dictionary<TKey, TValue> dictionary;

        protected Func<TValue, TKey> idSelector;

        public IReadOnlyList<TValue> List => list;

        public DictionaryList(Func<TValue, TKey> idSelector)
        {
            this.idSelector = idSelector;
        }

        public void Set(IEnumerable<TValue> list)
        {
            this.list = list.ToList();
            dictionary = null;
        }

        public void Add(TValue value)
        {
            list.Add(value);
            if (dictionary != null)
            {
                dictionary.Add(idSelector(value), value);
            }
        }

        public void Remove(TKey key)
        {
            var value = dictionary[key];
            list.Remove(value);
            if (dictionary != null)
            {
                dictionary.Remove(key);
            }
        }

        public void Remove(TValue value)
        {
            var key = idSelector(value);
            list.Remove(value);
            if (dictionary != null)
            {
                dictionary.Remove(key);
            }
        }

        public void Clear()
        {
            list.Clear();
            dictionary = null;
        }

        public TValue Get(TKey key)
        {
            InitializeIfNull();
            Assert.IsTrue(dictionary.ContainsKey(key), $"key={key}");
            return dictionary[key];
        }

        public bool ContainsKey(TKey key)
        {
            InitializeIfNull();
            return dictionary.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            InitializeIfNull();
            return dictionary.TryGetValue(key, out value);
        }

        private void InitializeIfNull()
        {
            // UnityEditorの場合は毎回初期化する
#if UNITY_EDITOR
            dictionary = null;
#endif
            if (dictionary == null)
            {
                dictionary = new Dictionary<TKey, TValue>();
                foreach (var item in list)
                {
                    dictionary.Add(idSelector(item), item);
                }
            }
        }
    }
}