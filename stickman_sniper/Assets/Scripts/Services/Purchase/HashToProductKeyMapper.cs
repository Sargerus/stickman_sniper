using System;
using System.Collections.Generic;
using UnityEngine;

namespace Purchase
{
    [CreateAssetMenu(menuName = "[Purchase] Purchase/HashToProductKeyMapper", fileName = "new HashToProductKeyMapper")]
    public class HashToProductKeyMapper : ScriptableObject
    {
        private Dictionary<string, string> HashToProductKey = new();

        public List<Values> Values = new();        

        public string GetProductKeyByHash(string hash)
        {
            if (string.IsNullOrEmpty(hash))
                return null;

            if (HashToProductKey.Count == 0)
            {
                foreach (var v in Values)
                    HashToProductKey.Add(v.Key, v.MappedValue);
            }

            string result = string.Empty;
            HashToProductKey.TryGetValue(hash, out result);
            return result;
        }
    }

    [Serializable]
    public class Values
    {
        public string Key;
        public string MappedValue;

        public Values(string key, string mappedValue)
        {
            Key = key;
            MappedValue = mappedValue;
        }
    }
}