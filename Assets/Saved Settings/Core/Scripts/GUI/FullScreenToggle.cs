using UnityEngine;
using UnityEngine.UI;

namespace SavedSettings.GUI
{
    /// <summary>
    /// Syncs the toggle value to the full screen setting.
    /// </summary>
    public class FullScreenToggle : BaseUILoadSetting
    {
        void Start()
        {
            Toggle toggle = GetComponent<Toggle>();
            LoadValue();
            toggle.onValueChanged.AddListener(delegate { Screen.fullScreen = toggle.isOn; });
        }

        public override void LoadValue()
        {
            GetComponent<Toggle>().isOn = Screen.fullScreen;
        }
    }
}
