using System;
using System.Collections.Generic;
using System.Text;

namespace CommonExtensions
{
    public static class DictionaryExtensions
    {
        // System.Collections.Generic.CollectionExtensions are not part of NETStandard??

        public static TValue GetValueOrDefault<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary, 
            TKey key, 
            TValue defaultValue = default(TValue))
        {
            if (dictionary.TryGetValue(key, out var value))
            {
                return value;
            }

            return defaultValue;
        }
    }
}
