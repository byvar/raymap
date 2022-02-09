using System.Collections.Generic;

namespace BinarySerializer.Unity {
    /// <summary>
    /// Extension methods for <see cref="Dictionary{TKey,TValue}"/>
    /// </summary>
    public static class DictionaryExtensions {
        /// <summary>
        /// Tries to get an item from the dictionary, returning the specified default one if not found
        /// </summary>
        /// <typeparam name="TKey">The key type</typeparam>
        /// <typeparam name="TValue">The value type</typeparam>
        /// <param name="dictionary">The dictionary</param>
        /// <param name="key">The key</param>
        /// <param name="defaultValue">The default value</param>
        /// <returns>The value</returns>
        public static TValue TryGetItem<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default) {
            return dictionary.TryGetValue(key, out TValue value) ? value : defaultValue;
        }
    }
}