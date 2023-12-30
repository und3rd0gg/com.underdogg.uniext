using System;
using UnityEngine;

namespace UniExt.Dictionary {
    [Serializable]
    public struct DictEntry<TKey, TValue>
    {
        [SerializeField] public TKey   Key;
        [SerializeField] public TValue Value;
    }
}