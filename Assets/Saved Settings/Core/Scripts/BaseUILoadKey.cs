using UnityEngine;

namespace SavedSettings
{
    /// <summary>
    /// Base class used for all UI elements that display key bindings.
    /// </summary>
    public abstract class BaseUILoadKey : MonoBehaviour
    {
        /// <summary>
        /// Called by ResetKeys and DiscardKeys after the settings are changed.
        /// </summary>
        public abstract void LoadValue();
    }
}