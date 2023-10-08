using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SavedSettings.GUI
{
    /// <summary>
    /// Syncs the toggle with the soft particles setting.
    /// </summary>
    [RequireComponent(typeof(Dropdown))]
    public class ShadowResolutionDropdown : BaseUILoadSetting
    {
        void Start()
        {
            Dropdown dropdown = GetComponent<Dropdown>();
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            options.Add(new Dropdown.OptionData("Low"));
            options.Add(new Dropdown.OptionData("Medium"));
            options.Add(new Dropdown.OptionData("High"));
            options.Add(new Dropdown.OptionData("Very High"));
            dropdown.options = options;
            LoadValue();
            dropdown.onValueChanged.AddListener(delegate { QualitySettings.shadowResolution = (ShadowResolution)dropdown.value; });
        }

        public override void LoadValue()
        {
            GetComponent<Dropdown>().value = (int)QualitySettings.shadowResolution;
        }
    }
}
