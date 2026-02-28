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
    public enum DuplicateKeyPolicy
    {
      Throw,
      KeepFirst,
      KeepLast
    }

    [SerializeField] private List<DictEntry<TKey, TValue>> _entries = new();
    [SerializeField] private DuplicateKeyPolicy _duplicateKeyPolicy = DuplicateKeyPolicy.KeepLast;
    [SerializeField] private bool _logValidationWarnings = true;

    private Dictionary<TKey, TValue> _cache = new();
    private Dictionary<TKey, int> _entryIndexMap = new();
    private bool _isInitialized;

    public DuplicateKeyPolicy DeserializationDuplicateKeyPolicy {
      get => _duplicateKeyPolicy;
      set => _duplicateKeyPolicy = value;
    }

    public bool LogValidationWarnings {
      get => _logValidationWarnings;
      set => _logValidationWarnings = value;
    }

    public TValue this[TKey key] {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get {
        EnsureInitialized();
        ValidateKeyNotNull(key);
        return _cache[key];
      }
      set => SetOrReplace(key, value);
    }

    public ICollection<TKey> Keys {
      get {
        EnsureInitialized();
        return _cache.Keys;
      }
    }

    public ICollection<TValue> Values {
      get {
        EnsureInitialized();
        return _cache.Values;
      }
    }

    public int Count {
      get {
        EnsureInitialized();
        return _cache.Count;
      }
    }

    public bool IsReadOnly => false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(TKey key, TValue value) {
      EnsureInitialized();
      ValidateKeyNotNull(key);

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
    public bool ContainsKey(TKey key) {
      EnsureInitialized();
      ValidateKeyNotNull(key);
      return _cache.ContainsKey(key);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetValue(TKey key, out TValue value) {
      EnsureInitialized();
      ValidateKeyNotNull(key);
      return _cache.TryGetValue(key, out value);
    }

    public bool Remove(TKey key) {
      EnsureInitialized();
      ValidateKeyNotNull(key);

      if (!_cache.Remove(key))
        return false;

      for (int i = _entries.Count - 1; i >= 0; i--) {
        if (!EqualityComparer<TKey>.Default.Equals(_entries[i].Key, key))
          continue;

        _entries.RemoveAt(i);
      }

      RebuildFromEntries();
      return true;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item) {
      EnsureInitialized();
      ValidateKeyNotNull(item.Key);

      if (!_cache.TryGetValue(item.Key, out var value) ||
          !EqualityComparer<TValue>.Default.Equals(value, item.Value))
        return false;

      return Remove(item.Key);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(KeyValuePair<TKey, TValue> item) {
      EnsureInitialized();
      ValidateKeyNotNull(item.Key);

      return _cache.TryGetValue(item.Key, out var value)
             && EqualityComparer<TValue>.Default.Equals(value, item.Value);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
      EnsureInitialized();

      if (array == null)
        throw new ArgumentNullException(nameof(array));

      if (arrayIndex < 0)
        throw new ArgumentOutOfRangeException(nameof(arrayIndex));

      if (arrayIndex > array.Length)
        throw new ArgumentException("arrayIndex is greater than the destination array length.", nameof(arrayIndex));

      if (array.Length - arrayIndex < _cache.Count)
        throw new ArgumentException("The destination array has insufficient space.", nameof(array));

      foreach (var pair in _cache)
        array[arrayIndex++] = pair;
    }

    public void Clear() {
      EnsureInitialized();

      _cache.Clear();
      _entries.Clear();
      _entryIndexMap.Clear();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryAdd(TKey key, TValue value) {
      EnsureInitialized();
      ValidateKeyNotNull(key);

      if (!_cache.TryAdd(key, value))
        return false;

      _entries.Add(new DictEntry<TKey, TValue> { Key = key, Value = value });
      _entryIndexMap[key] = _entries.Count - 1;
      return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetOrReplace(TKey key, TValue value) {
      EnsureInitialized();
      ValidateKeyNotNull(key);

      if (_cache.TryAdd(key, value)) {
        _entries.Add(new DictEntry<TKey, TValue> { Key = key, Value = value });
        _entryIndexMap[key] = _entries.Count - 1;
      } else {
        _cache[key] = value;
        UpdateSerializedValue(key, value);
      }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
      EnsureInitialized();
      return _cache.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public static explicit operator Dictionary<TKey, TValue>(UniDict<TKey, TValue> uniDict) {
      uniDict.EnsureInitialized();
      return new Dictionary<TKey, TValue>(uniDict._cache);
    }

    public static explicit operator UniDict<TKey, TValue>(Dictionary<TKey, TValue> dictionary) {
      if (dictionary == null)
        throw new ArgumentNullException(nameof(dictionary));

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
      RebuildFromEntries();
    }

    public void RebuildFromEntries() {
      EnsureStorage();
      _isInitialized = false;

      _cache.Clear();
      _entryIndexMap.Clear();
      for (int i = 0; i < _entries.Count; i++) {
        var entry = _entries[i];

        if (entry.Key is null) {
          LogValidationWarning($"Ignored null key at index {i} in UniDict<{typeof(TKey).Name}, {typeof(TValue).Name}>.");
          continue;
        }

        switch (_duplicateKeyPolicy) {
          case DuplicateKeyPolicy.Throw:
            if (_cache.ContainsKey(entry.Key))
              throw new ArgumentException($"Duplicate key '{entry.Key}' found at index {i} in UniDict<{typeof(TKey).Name}, {typeof(TValue).Name}>.");

            _cache.Add(entry.Key, entry.Value);
            _entryIndexMap[entry.Key] = i;
            break;
          case DuplicateKeyPolicy.KeepFirst:
            if (_cache.ContainsKey(entry.Key)) {
              LogValidationWarning($"Duplicate key '{entry.Key}' found at index {i}. Keeping the first value.");
              break;
            }

            _cache.Add(entry.Key, entry.Value);
            _entryIndexMap[entry.Key] = i;
            break;
          case DuplicateKeyPolicy.KeepLast:
            if (_cache.ContainsKey(entry.Key))
              LogValidationWarning($"Duplicate key '{entry.Key}' found at index {i}. Keeping the last value.");

            _cache[entry.Key] = entry.Value;
            _entryIndexMap[entry.Key] = i;
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }

      _isInitialized = true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EnsureInitialized() {
      if (_isInitialized)
        return;

      RebuildFromEntries();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EnsureStorage() {
      _entries ??= new List<DictEntry<TKey, TValue>>();
      _cache ??= new Dictionary<TKey, TValue>();
      _entryIndexMap ??= new Dictionary<TKey, int>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ValidateKeyNotNull(TKey key) {
      if (key is null)
        throw new ArgumentNullException(nameof(key));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void LogValidationWarning(string message) {
      if (!_logValidationWarnings)
        return;

#if UNITY_EDITOR
      if (!Application.isPlaying)
        return;
#endif

      Debug.LogWarning(message);
    }
  }
}
