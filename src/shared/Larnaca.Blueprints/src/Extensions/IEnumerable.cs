using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Larnaca.Blueprints
{
    public static class IEnumerableExtensions
    {
        public static bool IsNullOrEmpty(this IEnumerable target)
        {
            if (target == null)
            {
                return true;
            }

            // Optimization Note:
            // Since we don't know the implementation details of the collection we have to go with the safest and fastest check
            // without relying on .Count()
            return !target.GetEnumerator().MoveNext();
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> target)
        {
            if (target == null)
            {
                return true;
            }

            // Optimization Note:
            // Since we don't know the implementation details of the collection we have to go with the safest and fastest check
            // without relying on .Count()
            using var enumerator = target.GetEnumerator();
            return !enumerator.MoveNext();
        }

        /// <summary>
        /// Checks that the collection is not null and contains at least one element.
        /// Unlike .Any() will NOT throw null exception
        /// A wrapper for !.IsNullOrEmpty() extension
        /// </summary>
        public static bool IsAny<T>(this IEnumerable<T> target)
        {
            return !target.IsNullOrEmpty();
        }

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> target)
        {
            if (target == null)
            {
                // optimization note: Enumerable.Empty contains a static, empty, enumeration,
                // it's not instantiated per call, so more optimized than new[]
                return Enumerable.Empty<T>();
            }
            else
            {
                return target;
            }
        }
    }
}