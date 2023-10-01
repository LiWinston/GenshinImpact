using UnityEngine;

namespace Utility
{
    public static class Find
    {
        /// <summary>
        /// Utility function used to seek into a subclass for a Transform
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Transform FindDeepChild(Transform parent, string name)
        {
            Transform result = parent.Find(name);
            if (result != null)
            {
                return result;
            }

            foreach (Transform child in parent)
            {
                result = FindDeepChild(child, name);
                if (result != null)
                {
                    return result;
                }
            }

            return null; // 没有找到
        }
    }
}