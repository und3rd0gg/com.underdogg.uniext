using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace com.underdogg.uniext.Runtime.Dictionary
{
  [Serializable]
  public sealed class UniDict<TKey, TValue> : IDictionary<TKey, TValue>, ISerializationCallbackReceiver where TKey : notnull
  {
    [SerializeField] private List<DictEntry<TKey, TValue>> _entries = new();

    private Dictionary<TKey, TValue> _cache = new();
    private Dictionary<TKey, int> _entryIndexMap = new();

    public TValue this[TKey key] {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => _cache[key];
      set {
        if (_cache.TryAdd(key, value)) {
          _entries.Add(new DictEntry<TKey, TValue> { Key = key, Value = value });
          _entryIndexMap[key] = _entries.Count - 1;
        } else {
          _cache[key] = value;
          UpdateSerializedValue(key, value);
        }
      }
    }

    public ICollection<TKey> Keys => _cache.Keys;
    public ICollection<TValue> Values => _cache.Values;
    public int Count => _cache.Count;
    public bool IsReadOnly => false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(TKey key, TValue value) {
      if (!_cache.TryAdd(key, value))
        throw new ArgumentException($"An element with the key '{key}' already exists.");

      _entries.Add(new DictEntry<TKey, TValue> { Key = key, Value = value });
      _entryIndexMap[key] = _entries.Count - 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(KeyValuePair<TKey, TValue> item) {
      Add(item.Key, item.Value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ContainsKey(TKey key) => _cache.ContainsKey(key);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetValue(TKey key, out TValue value) => _cache.TryGetValue(key, out value);

    public bool Remove(TKey key) {
      if (!_cache.Remove(key))
        return false;

      if (_entryIndexMap.TryGetValue(key, out var index)) {
        int lastIndex = _entries.Count - 1;

        if (index != lastIndex) {
          var moved = _entries[lastIndex];
          _entries[index] = moved;
          _entryIndexMap[moved.Key] = index;
        }

        _entries.RemoveAt(lastIndex);
        _entryIndexMap.Remove(key);
      }

      return true;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item) {
      if (!_cache.TryGetValue(item.Key, out var value) ||
          !EqualityComparer<TValue>.Default.Equals(value, item.Value))
        return false;

      return Remove(item.Key);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(KeyValuePair<TKey, TValue> item) {
      return _cache.TryGetValue(item.Key, out var value)
             && EqualityComparer<TValue>.Default.Equals(value, item.Value);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
      foreach (var pair in _cache)
        array[arrayIndex++] = pair;
    }

    public void Clear() {
      _cache.Clear();
      _entries.Clear();
      _entryIndexMap.Clear();
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _cache.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public static explicit operator Dictionary<TKey, TValue>(UniDict<TKey, TValue> uniDict) {
      return new Dictionary<TKey, TValue>(uniDict._cache);
    }

    public static explicit operator UniDict<TKey, TValue>(Dictionary<TKey, TValue> dictionary) {
      var uniDict = new UniDict<TKey, TValue>();
      foreach (var pair in dictionary)
        uniDict.Add(pair.Key, pair.Value);
      return uniDict;
    }

    private void UpdateSerializedValue(TKey key, TValue value) {
      if (_entryIndexMap.TryGetValue(key, out var index))
        _entries[index] = new DictEntry<TKey, TValue> { Key = key, Value = value };
    }

    public void OnAfterDeserialize() {
      RebuildFromEntries();
    }

    public void OnBeforeSerialize() {
      _entries.Clear();
      _entryIndexMap.Clear();

      int i = 0;
      foreach (var pair in _cache) {
        _entries.Add(new DictEntry<TKey, TValue> { Key = pair.Key, Value = pair.Value });
        _entryIndexMap[pair.Key] = i++;
      }
    }

    public void RebuildFromEntries() {
      _cache.Clear();
      _entryIndexMap.Clear();

      for (int i = 0; i < _entries.Count; i++) {
        var entry = _entries[i];
        _cache[entry.Key] = entry.Value;
        _entryIndexMap[entry.Key] = i;
      }
    }
  }
}