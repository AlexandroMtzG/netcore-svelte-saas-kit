using System.Collections.Generic;

namespace NetcoreSaas.Application.Extensions
{
    public static class CollectionHelper
    {
        public static IDictionary<TKey, TValue> AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dic, IDictionary<TKey, TValue> dicToAdd)
        {
            foreach (var (key, value) in dicToAdd)
            {
                if (dic.ContainsKey(key) == false)
                {
                    dic.Add(key, value);
                }
            }
            return dic;
        }
    }
}