using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Larnaca.Blueprints
{
    public static class IDictionaryExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default)
        {
            if (dictionary == null) { throw new ArgumentNullException(nameof(dictionary)); }
            if (key == null) { throw new ArgumentNullException(nameof(key)); }
            return dictionary.TryGetValue(key, out TValue value) ? value : defaultValue;
        }

        public static TValue GetValueOrEmpty<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) where TValue : new()
        {
            if (dictionary == null) { throw new ArgumentNullException(nameof(dictionary)); }
            if (key == null) { throw new ArgumentNullException(nameof(key)); }

            //if the value exists in the dictionary and is NULL, we will return the default instance of it
            if (dictionary.TryGetValue(key, out TValue value))
            {
                return value != null ? value : new TValue();
            }
            return new TValue();
        }

        /// <summary>
        /// Adds the value to the indexed list. If the list doesn't exist, it will be created
        /// Returns the list to which the value is added
        /// </summary>
        public static TList AddToList<TKey, TValue, TList>(this IDictionary<TKey, TList> target, TKey key, params TValue[] values)
            where TList : ICollection<TValue>, new() => AddToList(target, key, valuesCollection: values);
        /// <summary>
        /// Adds the value to the indexed list. If the list doesn't exist, it will be created
        /// Returns the list to which the value is added
        /// </summary>
        public static TList AddToList<TKey, TValue, TList>(this IDictionary<TKey, TList> target, TKey key, IEnumerable<TValue> valuesCollection)
            where TList : ICollection<TValue>, new()
        {
            if (target.TryGetValue(key, out TList theReturn))
            {
            }
            else
            {
                theReturn = new TList();
                target.Add(key, theReturn);
            }
            foreach (var currentValue in valuesCollection)
            {
                theReturn.Add(currentValue);
            }
            return theReturn;
        }


        /// <summary>
        /// Adds the value to the indexed list. If the list doesn't exist, it will be created
        /// Returns the list to which the value is added
        /// </summary>
        public static TList AddToListWithConcurrency<TKey, TValue, TList>(this IDictionary<TKey, TList> target, TKey key, params TValue[] values)
            where TList : ICollection<TValue>, new() => AddToListWithConcurrency(target, key, valuesCollection: values);
        /// <summary>
        /// Adds the value to the indexed list. If the list doesn't exist, it will be created
        /// Returns the list to which the value is added
        /// </summary>
        public static TList AddToListWithConcurrency<TKey, TValue, TList>(this IDictionary<TKey, TList> target, TKey key, IEnumerable<TValue> valuesCollection)
            where TList : ICollection<TValue>, new()
        {
            TList theReturn;
            if (target is ConcurrentDictionary<TKey, TList> concurrentDictionary)
            {
                if (!concurrentDictionary.TryGetValue(key, out theReturn))
                {
                    theReturn = new TList();
                    target.Add(key, theReturn);
                }
            }
            else
            {
                lock (target)
                {
                    if (!target.TryGetValue(key, out theReturn))
                    {
                        theReturn = new TList();
                        target.Add(key, theReturn);
                    }
                }
            }

            if (valuesCollection is ConcurrentBag<TValue>
                || valuesCollection is ConcurrentQueue<TValue>
                || valuesCollection is ConcurrentStack<TValue>)
            {
                foreach (var currentValue in valuesCollection)
                {
                    theReturn.Add(currentValue);
                }
            }
            else
            {
                lock (valuesCollection)
                {
                    foreach (var currentValue in valuesCollection)
                    {
                        theReturn.Add(currentValue);
                    }
                }
            }
            return theReturn;
        }
    }
}
