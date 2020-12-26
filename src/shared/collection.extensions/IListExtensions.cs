using System;
using System.Collections.Generic;

namespace collection.extensions
{
    public static class IListExtensions
    {
        /// <summary>
        ///     Searches for an element that matches the conditions defined by the specified
        ///     predicate, and returns the zero-based index of the first occurrence within the
        ///     entire System.Collections.Generic.IList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The System.Collections.Generic.IList to search.</param>
        /// <param name="match">The System.Predicate delegate that defines the conditions of the element to search for.</param>
        /// <returns>The zero-based index of the first occurrence of an element that matches the conditions defined by match, if found; otherwise, -1.</returns>
        public static int FindIndex<T>(this IList<T> items, Predicate<T> match)
        {
            if (items == null) throw new ArgumentNullException("items");
            if (match == null) throw new ArgumentNullException("predicate");

            var retVal = 0;

            foreach (var item in items)
            {
                if (match(item)) return retVal;
                retVal++;
            }

            return -1;
        }

        /// <summary>
        ///     Searches for all elements that match the conditions defined by the specified
        ///     predicate, and returns the zero-based index list of the occurrences within the
        ///     entire System.Collections.Generic.IList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The System.Collections.Generic.IList to search.</param>
        /// <param name="match">The System.Predicate delegate that defines the conditions of the elements to search for.</param>
        /// <returns>The zero-based index list of the elements that match the conditions defined by match, if found.</returns>
        public static List<int> FindIndexes<T>(this IList<T> items, Predicate<T> match)
        {
            if (items == null) throw new ArgumentNullException("items");
            if (match == null) throw new ArgumentNullException("predicate");

            var indexes = new List<int>();
            var elementIndex = 0;

            foreach (var item in items)
            {
                if (match(item))
                {
                    indexes.Add(elementIndex);
                }

                elementIndex++;
            }

            return indexes;
        }

        ///<summary>Removes the first occurrence of an element that matches the conditions defined by the specified predicate from the System.Collections.Generic.IList.</summary>
        ///<param name="items">The System.Collections.Generic.IList to search.</param>
        ///<param name="match">The System.Predicate delegate that defines the conditions of the element to remove.</param>
        ///<returns>True if item is successfully removed; otherwise, false. This method also returns false if item was not found in the System.Collections.Generic.List.</returns>
        public static bool RemoveFirst<T>(this IList<T> items, Predicate<T> match)
        {
            var index = FindIndex(items, match);

            if (index >= 0)
            {
                items.RemoveAt(index);

                return true;
            }

            return false;
        }

        /// <summary>
        ///     Removes all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The System.Collections.Generic.IList to search.</param>
        /// <param name="match">The System.Predicate delegate that defines the conditions of the elements to remove.</param>
        /// <returns>The number of elements removed from the System.Collections.Generic.IList.</returns>
        public static int RemoveAll<T>(this IList<T> items, Predicate<T> match)
        {
            var indexes = FindIndexes(items, match);

            for (var i = indexes.Count - 1; i >= 0; i--)
            {
                items.RemoveAt(indexes[i]);
            }

            return indexes.Count;
        }
    }
}
