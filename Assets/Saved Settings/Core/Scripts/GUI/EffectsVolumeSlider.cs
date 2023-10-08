using UnityEngine;
using UnityEngine.UI;

namespace SavedSettings.GUI
{
    /// <summary>
    /// Syncs the slider value to the sound effects volume.
    /// </summary>
    [RequireComponent(typeof(Slider))]
    public class EffectsVolumeSlider : BaseUILoadSetting
    {
        void Start()
        {
            Slider slider = GetComponent<Slider>();
            LoadValue();
            slider.onValueChanged.AddListener(delegate { SettingsHelper.SoundEffectVolume = slider.value; });
        }

        public override void LoadValue()
        {
            GetComponent<Slider>().value = SettingsHelper.SoundEffectVolume;
        }
    }
}
