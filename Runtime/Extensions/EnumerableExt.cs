using System;
using System.Collections.Generic;

namespace com.underdogg.uniext.Runtime.Extensions
{
    public static class EnumerableExt
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            foreach (var element in enumerable)
            {
                action(element);
            }

            return enumerable;
        }
    }
}
