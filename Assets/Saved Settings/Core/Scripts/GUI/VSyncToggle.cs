using UnityEngine;
using UnityEngine.UI;

namespace SavedSettings.GUI
{
    /// <summary>
    /// Syncs the toggle value to the vsync setting.
    /// </summary>
    [RequireComponent(typeof(Toggle))]
    public class VSyncToggle : BaseUILoadSetting
    {
        void Start()
        {
            Toggle toggle = GetComponent<Toggle>();
            LoadValue();
            toggle.onValueChanged.AddListener(delegate { SettingsHelper.VSync = toggle.isOn; });
        }

        public override void LoadValue()
        {
            GetComponent<Toggle>().isOn = SettingsHelper.VSync;
        }
    }
}