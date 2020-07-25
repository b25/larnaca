using System.Collections.Generic;
using System.Linq;

namespace mssql.collector
{
    internal static class IEnumerableUtils
    {
        public static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }
    }
}