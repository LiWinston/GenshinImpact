using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SavedSettings.GUI
{
    /// <summary>
    /// Syncs the toggle with the soft particles setting.
    /// </summary>
    [RequireComponent(typeof(Dropdown))]
    public class ShadowQualityDropdown : BaseUILoadSetting
    {
        void Start()
        {
            Dropdown dropdown = GetComponent<Dropdown>();
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            options.Add(new Dropdown.OptionData("No Shadows"));
            options.Add(new Dropdown.OptionData("Hard Shadows"));
            options.Add(new Dropdown.OptionData("Soft Shadows"));
            dropdown.options = options;
            LoadValue();
            dropdown.onValueChanged.AddListener(delegate { QualitySettings.shadows = (ShadowQuality)dropdown.value; });
        }

        public override void LoadValue()
        {
            GetComponent<Dropdown>().value = (int)QualitySettings.shadows;
        }
    }
}
