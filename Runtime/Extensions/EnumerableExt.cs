using System;
using System.Collections.Generic;

namespace UniExt.Extensions
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