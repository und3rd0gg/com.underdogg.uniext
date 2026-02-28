using System;
using UnityEngine;

namespace com.underdogg.uniext.Runtime.Extensions
{
    public static class DataExt
    {
        public static string ToJson(this object obj, bool prettyPrint = false)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            return JsonUtility.ToJson(obj, prettyPrint);
        }

        public static T ToDeserialized<T>(this string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentException("JSON payload is empty.", nameof(json));

            return JsonUtility.FromJson<T>(json);
        }
    }
}
