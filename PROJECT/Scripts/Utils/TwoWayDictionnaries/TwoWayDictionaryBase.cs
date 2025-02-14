using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Author : Camille SMOLARSKI

namespace Com.IsartDigital.WoolyWay.Utils.TwoWayDictionnaries
{
    /// <summary>
    /// Base class for two-way dictionaries.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public abstract class TwoWayDictionaryBase<TKey, TValue> : ICollection
        where TKey : notnull
        where TValue : notnull
    {
        public bool IsSynchronized => false;
        public object SyncRoot => this;

        public int Count => pairs.Length;

        //returns an array containing all the keys when myDict.Keys is called.
        public TKey[] Keys {
            get {
                TKey[] lArray = new TKey[pairs.Length];
                for (int i = 0; i < pairs.Length; i++)
                    lArray[i] = pairs[i].Key;
                return lArray;
            }
        }

        //returns an array containing all the values when myDict.Values is called.
        public TValue[] Values
        {
            get
            {
                TValue[] lArray = new TValue[pairs.Length];
                for (int i = 0; i < pairs.Length; i++)
                    lArray[i] = pairs[i].Value;
                return lArray;
            }
        }

        //Initializing array contining all the dictionnary's values.
        protected KeyValuePair<TKey, TValue>[] pairs = Array.Empty<KeyValuePair<TKey, TValue>>();

        //You might not be familiar with this writing. Its to return a value when myDict[object] is called. Like you would do with a List.
        public TValue this[TKey pKey] => FindPair(pKey).Value;
        public TKey this[TValue pValue] => FindPair(pValue).Key;
        public KeyValuePair<TKey, TValue> this[int pIndex] => pairs[pIndex];

        /// <summary></summary>
        /// <param name="pKey"></param>
        /// <returns>true if it contains the key. Otherwise, false.</returns>
        public bool Contains(TKey pKey)
        {
            return Keys.Contains(pKey);
        }

        /// <summary></summary>
        /// <param name="pValue"></param>
        /// <returns>true if it contains the value. Otherwise, false.</returns>
        public bool Contains(TValue pValue)
        {
            return Values.Contains(pValue);
        }

        /// <summary></summary>
        /// <param name="pValue"></param>
        /// <returns>true if it contains the pair. Otherwise, false.</returns>
        public bool Contains(KeyValuePair<TKey, TValue> pPair)
        {
            return pairs.Contains(pPair);
        }

        /// <summary></summary>
        /// <param name="pKey"></param>
        /// <returns>The corresponding key/value pair.</returns>
        /// <exception cref="KeyNotFoundException"></exception>
        protected KeyValuePair<TKey, TValue> FindPair(TKey pKey)
        {
            if (Contains(pKey))
                return Array.Find(pairs, (KeyValuePair<TKey, TValue> pPair) => pPair.Key.Equals(pKey));
            throw new KeyNotFoundException();
        }


        /// <summary>
        /// </summary>
        /// <param name="pKey"></param>
        /// <returns>The corresponding key/value pair.</returns>
        /// <exception cref="KeyNotFoundException"></exception>
        protected KeyValuePair<TKey, TValue> FindPair(TValue pValue)
        {
            if (Contains(pValue))
                return Array.Find(pairs, (KeyValuePair<TKey, TValue> pPair) => pPair.Value.Equals(pValue));
            throw new KeyNotFoundException(nameof(pValue));
        }

        /// <summary></summary>
        /// <returns>A key/value pair array containing all of this dictionary's pairs.</returns>
        public KeyValuePair<TKey, TValue>[] ToArray()
        {
            return pairs;
        }

        /*
         * INHERITED METHODS
         */
        public void CopyTo(Array pArray, int pIndex)
        {
            if(pArray is KeyValuePair<TKey, TValue>[])
            {
                pairs.CopyTo(pArray, pIndex);
            }
            throw new ArrayTypeMismatchException();
        }

        public IEnumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <summary>
        /// Enumerator struct that can be used in foreach loops.
        /// </summary>
        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDictionaryEnumerator
        {
            public KeyValuePair<TKey, TValue> Current => dictionary[index];
            public DictionaryEntry Entry => new(Current.Key, Current.Value);
            public object Key => Current.Key;
            public object Value => Current.Value;
            object IEnumerator.Current => Current;

            private TwoWayDictionaryBase<TKey, TValue> dictionary;
            private int index = -1;

            internal Enumerator(TwoWayDictionaryBase<TKey, TValue> pDictionnary)
            {
                dictionary = pDictionnary;
            }

            public bool MoveNext()
            {
                if (index < dictionary.pairs.Length - 1)
                {
                    index++;
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                index = -1;
            }

            public void Dispose() { }
        }
    }
}
