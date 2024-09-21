using System;
using System.Collections.Generic;

namespace com.underdogg.uniext.Runtime.Extensions
{
    public static class EnumerableExt
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (T element in enumerable)
            {
                action(element);
            }

            return enumerable;
        }
    }
}