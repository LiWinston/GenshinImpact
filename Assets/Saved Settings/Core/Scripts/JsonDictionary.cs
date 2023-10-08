using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SavedSettings
{
    /// <summary>
    /// Helper class that allows you to save dictionaries to a json format.
    /// </summary>
    public static class JsonDictionary
    {
        /// <summary>
        /// Converts a dictionary to json string.
        /// The type's used by the dictionary must be serializable.
        /// </summary>
        public static string dictionaryToJson<A, B>(Dictionary<A, B> dictionary)
        {
            try
            {
                JsonDictionaryWrapper<A, B> jsonWrapper = new JsonDictionaryWrapper<A, B>();
                jsonWrapper.arrayA = dictionary.Keys.ToArray<A>();
                jsonWrapper.arrayB = dictionary.Values.ToArray<B>();
                return JsonUtility.ToJson(jsonWrapper);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Converts a json string to a dictionary.
        /// The type's used by the dictionary must be serializable.
        /// </summary>
        public static Dictionary<A, B> jsonToDictionary<A, B>(string json)
        {
            try
            {
                JsonDictionaryWrapper<A, B> wrapper = JsonUtility.FromJson<JsonDictionaryWrapper<A, B>>(json);
                Dictionary<A, B> dictionary = new Dictionary<A, B> { };
                for (int i = 0; i < wrapper.arrayA.Length; ++i)
                {
                    dictionary.Add(wrapper.arrayA[i], wrapper.arrayB[i]);
                }
                return dictionary;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Json can save arrays as long as they are contained within an object.
        /// In this case, a dictionary is saved as 2 arrays.
        /// </summary>
        [System.Serializable]
        private class JsonDictionaryWrapper<A, B>
        {
            public A[] arrayA;
            public B[] arrayB;
        }
    }
}
