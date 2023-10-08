using UnityEngine;
using UnityEngine.UI;

namespace SavedSettings.GUI
{
    /// <summary>
    /// Syncs the slider value to the music volume.
    /// </summary>
    [RequireComponent(typeof(Slider))]
    public class MusicVolumeSlider : BaseUILoadSetting
    {
        void Start()
        {
            Slider slider = GetComponent<Slider>();
            LoadValue();
            slider.onValueChanged.AddListener(delegate { SettingsHelper.MusicVolume = slider.value; });
        }

        public override void LoadValue()
        {
            GetComponent<Slider>().value = SettingsHelper.MusicVolume;
        }
    }
}
