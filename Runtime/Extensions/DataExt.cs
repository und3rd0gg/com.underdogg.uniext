using UnityEngine;

namespace com.underdogg.uniext.Runtime.Extensions
{
    public static class DataExt
    {
        public static string ToJson(this object obj)
        {
            return JsonUtility.ToJson(obj);
        }

        public static T ToDeserialized<T>(this string json)
        {
            return JsonUtility.FromJson<T>(json);
        }
    }
}