using UnityEngine;
using UnityEngine.UI;

namespace SavedSettings.GUI
{
    /// <summary>
    /// Syncs the button to discarding the changed settings.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class DiscardSettingsButton : MonoBehaviour
    {
        [SerializeField] BaseUILoadSetting[] _SettingUIs;

        void Start()
        {
            GetComponent<Button>().onClick.AddListener(delegate
            {
                SettingsHelper.DiscardChanges();
                foreach (BaseUILoadSetting setting in _SettingUIs)
                {
                    if (setting != null)
                    {
                        setting.LoadValue();
                    }
                }
            });
        }

#if UNITY_EDITOR

        private void Reset()
        {
            _SettingUIs = FindObjectsOfType<BaseUILoadSetting>();
        }

#endif
    }
}
