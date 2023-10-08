using UnityEngine;

namespace SavedSettings
{
    /// <summary>
    /// Base class used for all UI elements that display settings.
    /// </summary>
    public abstract class BaseUILoadSetting : MonoBehaviour
    {
        /// <summary>
        /// Called by ResetSettings and DiscardSettings after the settings are changed.
        /// </summary>
        public abstract void LoadValue();
    }
}