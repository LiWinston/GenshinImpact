using UnityEngine;
using UnityEngine.UI;

namespace SavedSettings.GUI
{
    /// <summary>
    /// Syncs the slider value to the master volume.
    /// </summary>
    [RequireComponent(typeof(Slider))]
    public class MasterVolumeSlider : BaseUILoadSetting
    {
        void Start()
        {
            Slider slider = GetComponent<Slider>();
            LoadValue();
            slider.onValueChanged.AddListener(delegate { SettingsHelper.MasterVolume = slider.value; });
        }

        public override void LoadValue()
        {
            GetComponent<Slider>().value = SettingsHelper.MasterVolume;
        }
    }
}