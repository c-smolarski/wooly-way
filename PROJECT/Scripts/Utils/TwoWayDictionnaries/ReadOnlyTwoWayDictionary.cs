using System;
using System.Collections.Generic;

// Author : Camille SMOLARSKI

namespace Com.IsartDigital.WoolyWay.Utils.TwoWayDictionnaries
{
    /// <summary>
    /// <para>A dictionnary that can be read by entering key or value! Keys AND values must be unique.</para>
    /// <para>This is a read only dictionary, you can change the values but not add or remove.</para>
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class ReadOnlyTwoWayDictionary<TKey, TValue> : TwoWayDictionaryBase<TKey, TValue>
    {
        public ReadOnlyTwoWayDictionary(TwoWayDictionary<TKey, TValue> pDictionnary = default)
        {
            if (pDictionnary == default)
            {
                pairs = Array.Empty<KeyValuePair<TKey, TValue>>();
                return;
            }

            Array.Resize(ref pairs, pDictionnary.Count);
            for (int i = 0; i < pDictionnary.Count; i++)
                pairs[i] = pDictionnary[i];
        }
    }
}
