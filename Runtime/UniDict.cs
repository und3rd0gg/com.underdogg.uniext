using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniExt
{
    [Serializable]
    public class UniDict<TKey, TValue>
    {
        private readonly int ParallelCount;

        [SerializeField] public List<DictEntry<TKey, TValue>> Dictionary;

        public UniDict(int parallelCount = 100)
        {
            ParallelCount = 100;
            Dictionary    = new List<DictEntry<TKey, TValue>>();
        }

        public static explicit operator Dictionary<TKey, TValue>(UniDict<TKey, TValue> uniDict)
        {
            var dict = new Dictionary<TKey, TValue>();

            foreach (DictEntry<TKey, TValue> pair in uniDict.Dictionary)
            {
                dict[pair.Key] = pair.Value;
            }

            return dict;
        }

        public static explicit operator UniDict<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
        {
            var uniDict = new UniDict<TKey, TValue>();

            foreach (KeyValuePair<TKey, TValue> pair in dictionary)
            {
                uniDict.Dictionary.Add(new DictEntry<TKey, TValue> { Key = pair.Key, Value = pair.Value });
            }

            return uniDict;
        }

        public TValue this[TKey key]
        {
            get =>
                GetValue(key);
            set
            {
                if (!Dictionary.Any(x => x.Key.Equals(key)))
                {
                    Dictionary
                        .Add(new DictEntry<TKey, TValue> { Key = key, Value = value });
                }
                else
                {
                    DictEntry<TKey, TValue> entry = Dictionary
                        .Find(x => x.Key.Equals(key));
                    entry.Value = value;
                }
            }
        }

        public IEnumerable<TKey> Keys
        {
            get
            {
                if (Dictionary.Count >= ParallelCount)
                {
                    return Dictionary.Select(x => x.Key)
                                     .AsParallel();
                }
                else
                {
                    return Dictionary.Select(x => x.Key);
                }
            }
        }

        public IEnumerable<TValue> Values
        {
            get
            {
                if (Dictionary.Count >= ParallelCount)
                {
                    return Dictionary
                           .Select(x => x.Value)
                           .AsParallel();
                }
                else
                {
                    return Dictionary
                        .Select(x => x.Value);
                }
            }
        }

        public void Clear() =>
            Dictionary.Clear();

        public TValue GetValue(TKey key) =>
            Dictionary
                .Find(x => x.Key.Equals(key)).Value;

        public void AddEntry(TKey key, TValue value) =>
            Dictionary.Add(new DictEntry<TKey, TValue> { Key = key, Value = value });
    }

    [Serializable]
    public struct DictEntry<TKey, TValue>
    {
        [SerializeField] public TKey   Key;
        [SerializeField] public TValue Value;
    }
}