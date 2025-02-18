using System;
using System.Collections.Generic;

// Author : Camille SMOLARSKI

namespace Com.IsartDigital.WoolyWay.Utils.TwoWayDictionnaries
{
    /// <summary>
    /// A dictionnary that can be read by entering key or value! Keys AND values must be unique.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class TwoWayDictionary<TKey, TValue> : TwoWayDictionaryBase<TKey, TValue>
    {
        /// <summary>
        /// Adds the key/value pair to the dictionary. Both key AND value MUST be unique.
        /// </summary>
        /// <param name="pKey"></param>
        /// <param name="pValue"></param>
        /// <exception cref="ArgumentException"></exception>
        public void Add(TKey pKey, TValue pValue)
        {
            if (Contains(pKey))
                throw new ArgumentException($"{this} already contains {pKey}");
            else if (Contains(pValue))
                throw new ArgumentException($"{this} already contains {pValue}");

            Array.Resize(ref pairs, pairs.Length + 1);
            pairs[pairs.Length - 1] = new(pKey, pValue);

        }

        /// <summary>
        /// Adds the pair to the dictionary. Both key AND value MUST be unique.
        /// </summary>
        /// <param name="pPair"></param>
        /// <exception cref="ArgumentException"></exception>
        public void Add(KeyValuePair<TKey, TValue> pPair)
        {
            Add(pPair.Key, pPair.Value);
        }

        /// <summary>
        /// Removes the key/value pair from the dictionary.
        /// </summary>
        /// <param name="pPair"></param>
        /// <exception cref="Exception"></exception>
        public void Remove(KeyValuePair<TKey, TValue> pPair)
        {
            if (Contains(pPair))
            {
                KeyValuePair<TKey, TValue>[] lNewArray = new KeyValuePair<TKey, TValue>[pairs.Length - 1];
                int j = 0;
                for (int i = 0; i < lNewArray.Length; i++)
                {
                    if (pairs[j].Equals(pPair))
                        j++;
                    lNewArray[i] = pairs[j];
                    j++;
                }
                pairs = lNewArray;
            }
            else
                throw new Exception($"{pPair} not found in {this}");
        }

        /// <summary>
        /// Removes the key/value pair from the dictionary.
        /// </summary>
        /// <param name="pKey"></param>
        public void Remove(TKey pKey)
        {
            Remove(FindPair(pKey));
        }

        /// <summary>
        /// Removes the key/value pair from the dictionary.
        /// </summary>
        /// <param name="pValue"></param>
        public void Remove(TValue pValue)
        {
            Remove(FindPair(pValue));
        }

        /// <summary>
        /// Removes all key/value pairs from the dictionary.
        /// </summary>
        public void Clear()
        {
            pairs = Array.Empty<KeyValuePair<TKey, TValue>>();
        }

        /// <summary></summary>
        /// <returns>The same dictionnary but you can't add or remove elements.</returns>
        public ReadOnlyTwoWayDictionary<TKey, TValue> ToReadOnly()
        {
            return new ReadOnlyTwoWayDictionary<TKey, TValue>(this);
        }
    }
}
