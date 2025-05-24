using System;
using UnityEngine;

namespace com.underdogg.uniext.Runtime.Dictionary
{
  [Serializable]
  public struct DictEntry<TKey, TValue>
  {
    [SerializeField] public TKey Key;
    [SerializeField] public TValue Value;
  }
}