using UnityEngine;
using UnityEngine.UI;

namespace SavedSettings.GUI
{
    /// <summary>
    /// Syncs the button to resetting the changed settings.
    /// </summary>
    public class ResetSettingsButton : MonoBehaviour
    {
        [SerializeField] BaseUILoadSetting[] _SettingUIs;

        void Start()
        {
            GetComponent<Button>().onClick.AddListener(delegate
            {
                SettingsHelper.ResetData();
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

