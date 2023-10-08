using UnityEngine;
namespace SavedSettings
{
    /// <summary>
    /// Helper class that allows you to save arrays of objects to a json format.
    /// </summary>
    public static class JsonArray
    {
        /// <summary>
        /// Converts an array to a json string.
        /// If the conversion fails, null is returned.
        /// </summary>
        public static string arrayToJson<T>(T[] array)
        {
            try
            {
                JsonArrayWrapper<T> jsonWrapper = new JsonArrayWrapper<T>();
                jsonWrapper.array = array;
                return JsonUtility.ToJson(jsonWrapper);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Converts a json string to an array.
        /// If the conversion fails, null is returned.
        /// </summary>
        public static T[] jsonToArray<T>(string json)
        {
            try
            {
                JsonArrayWrapper<T> wrapper = JsonUtility.FromJson<JsonArrayWrapper<T>>(json);
                return wrapper.array;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Json can save arrays as long as they are contained within an object.
        /// </summary>
        [System.Serializable]
        private class JsonArrayWrapper<T>
        {
            public T[] array;
        }
    }
}
