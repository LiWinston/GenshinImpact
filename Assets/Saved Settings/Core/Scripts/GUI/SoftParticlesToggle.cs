using UnityEngine;
using UnityEngine.UI;

namespace SavedSettings.GUI
{
    /// <summary>
    /// Syncs the toggle with the soft particles setting.
    /// </summary>
    [RequireComponent(typeof(Toggle))]
    public class SoftParticlesToggle : BaseUILoadSetting
    {
        void Start()
        {
            Toggle toggle = GetComponent<Toggle>();
            LoadValue();
            toggle.onValueChanged.AddListener(delegate { QualitySettings.softParticles = toggle.isOn; });
        }

        public override void LoadValue()
        {
            GetComponent<Toggle>().isOn = QualitySettings.softParticles;
        }
    }
}
