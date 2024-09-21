using System.Collections.Generic;
using UnityEngine;

namespace com.underdogg.uniext.Runtime.Extensions {
    public static class ListExt {
        public static T Last<T>(this List<T> list) {
            return list[^1];
        }

        public static T Last<T>(this T[] array) {
            return array[^1];
        }

        public static T Peek<T>(this List<T> list) {
            return list[0];
        }

        public static T Peek<T>(this T[] array) {
            return array[0];
        }

        public static T Dequeue<T>(this List<T> list) {
            var item = list[0];
            list.RemoveAt(0);
            return item;
        }

        public static T Trim<T>(this List<T> list) {
            var item = list.Last();
            list.Remove(item);
            return item;
        }

        public static T GetRandomItem<T>(this List<T> list) {
            return list[Random.Range(0, list.Count)];
        }

        public static T GetRandomItem<T>(this T[] list) {
            return list[Random.Range(0, list.Length)];
        }

        public static T KeyByValue<T, W>(this Dictionary<T, W> dict, W val) {
            T key = default;
            foreach (var pair in dict)
                if (EqualityComparer<W>.Default.Equals(pair.Value, val)) {
                    key = pair.Key;
                    break;
                }

            return key;
        }
    }
}