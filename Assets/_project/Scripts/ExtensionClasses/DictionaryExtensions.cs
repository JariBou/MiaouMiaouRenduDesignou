using System;
using System.Collections.Generic;

namespace _project.Scripts.ExtensionClasses
{
    public static class DictionaryExtensions
    {
        /// <summary>
        ///     Returns the value in the dictionary, if no value present creates it with the <paramref name="addAction" /> provided
        /// </summary>
        /// <param name="dictionary">The dictionary in which to perform the action</param>
        /// <param name="key">The key  of the dictionary</param>
        /// <param name="addAction">The action to call to create a value and add it at Key</param>
        /// <typeparam name="TKey">The type of the Key</typeparam>
        /// <typeparam name="TValue">The type of the value</typeparam>
        /// <returns>The value at key</returns>
        public static TValue TryAddAndGet<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key,
                                                        Func<TValue> addAction)
        {
            if (dictionary.TryGetValue(key, out TValue value)) return value;

            dictionary.Add(key, addAction.Invoke());
            return dictionary[key];
        }

        /// <summary>
        ///     Returns the value in the dictionary, if no value present creates it with the <paramref name="defaultValue" /> provided
        /// </summary>
        /// <param name="dictionary">The dictionary in which to perform the action</param>
        /// <param name="key">The key  of the dictionary</param>
        /// <param name="defaultValue">The default value to add at key if none found</param>
        /// <typeparam name="TKey">The type of the Key</typeparam>
        /// <typeparam name="TValue">The type of the value</typeparam>
        /// <returns>The value at key</returns>
        public static TValue TryAddAndGet<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key,
                                                        TValue defaultValue = default)
        {
            if (dictionary.TryGetValue(key, out TValue value)) return value;

            dictionary.Add(key, defaultValue);
            return dictionary[key];
        }
    }
}