using System;
using System.Collections.Generic;

namespace com.underdogg.uniext.Runtime.Extensions
{
    public static class ListExt
    {
        public static T Last<T>(this List<T> list)
        {
            ValidateListHasItems(list, nameof(list));
            return list[^1];
        }

        public static T Last<T>(this T[] array)
        {
            ValidateArrayHasItems(array, nameof(array));
            return array[^1];
        }

        public static T PeekFirst<T>(this List<T> list)
        {
            ValidateListHasItems(list, nameof(list));
            return list[0];
        }

        public static T PeekFirst<T>(this T[] array)
        {
            ValidateArrayHasItems(array, nameof(array));
            return array[0];
        }

        [Obsolete("Use PeekFirst(List<T>) instead.")]
        public static T Peek<T>(this List<T> list) => PeekFirst(list);

        [Obsolete("Use PeekFirst(T[]) instead.")]
        public static T Peek<T>(this T[] array) => PeekFirst(array);

        public static bool TryPeekFirst<T>(this List<T> list, out T value)
        {
            if (list == null || list.Count == 0)
            {
                value = default;
                return false;
            }

            value = list[0];
            return true;
        }

        public static T Dequeue<T>(this List<T> list)
        {
            ValidateListHasItems(list, nameof(list));
            var item = list[0];
            list.RemoveAt(0);
            return item;
        }

        public static T PopLast<T>(this List<T> list)
        {
            ValidateListHasItems(list, nameof(list));
            var lastIndex = list.Count - 1;
            var item = list[lastIndex];
            list.RemoveAt(lastIndex);
            return item;
        }

        public static bool TryPopLast<T>(this List<T> list, out T value)
        {
            if (list == null || list.Count == 0)
            {
                value = default;
                return false;
            }

            var lastIndex = list.Count - 1;
            value = list[lastIndex];
            list.RemoveAt(lastIndex);
            return true;
        }

        [Obsolete("Use PopLast(List<T>) instead.")]
        public static T Trim<T>(this List<T> list) => PopLast(list);

        public static T GetRandomItem<T>(this List<T> list)
        {
            ValidateListHasItems(list, nameof(list));
            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        public static T GetRandomItem<T>(this T[] array)
        {
            ValidateArrayHasItems(array, nameof(array));
            return array[UnityEngine.Random.Range(0, array.Length)];
        }

        [Obsolete("Use TryGetKeyByValue instead to avoid ambiguous default values.")]
        public static T KeyByValue<T, W>(this Dictionary<T, W> dict, W val)
        {
            if (TryGetKeyByValue(dict, val, out var key))
                return key;

            return default;
        }

        public static bool TryGetKeyByValue<T, W>(this IDictionary<T, W> dict, W value, out T key)
        {
            if (dict == null)
                throw new ArgumentNullException(nameof(dict));

            foreach (var pair in dict)
            {
                if (!EqualityComparer<W>.Default.Equals(pair.Value, value))
                    continue;

                key = pair.Key;
                return true;
            }

            key = default;
            return false;
        }

        private static void ValidateListHasItems<T>(List<T> list, string paramName)
        {
            if (list == null)
                throw new ArgumentNullException(paramName);

            if (list.Count == 0)
                throw new InvalidOperationException("Collection is empty.");
        }

        private static void ValidateArrayHasItems<T>(T[] array, string paramName)
        {
            if (array == null)
                throw new ArgumentNullException(paramName);

            if (array.Length == 0)
                throw new InvalidOperationException("Collection is empty.");
        }
    }
}
