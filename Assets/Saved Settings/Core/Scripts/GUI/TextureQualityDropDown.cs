using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SavedSettings.GUI
{
    /// <summary>
    /// Syncs the drop down with the master texture limit setting.
    /// </summary>
    [RequireComponent(typeof(Dropdown))]
    public class TextureQualityDropDown : BaseUILoadSetting
    {
        void Start()
        {
            Dropdown dropdown = GetComponent<Dropdown>();
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            options.Add(new Dropdown.OptionData("Full Resolution"));
            options.Add(new Dropdown.OptionData("1/2"));
            options.Add(new Dropdown.OptionData("1/4"));
            options.Add(new Dropdown.OptionData("1/8"));
            dropdown.options = options;
            LoadValue();
            dropdown.onValueChanged.AddListener(delegate { QualitySettings.globalTextureMipmapLimit = dropdown.value; });
        }

        public override void LoadValue()
        {
            GetComponent<Dropdown>().value = QualitySettings.globalTextureMipmapLimit;
        }
    }
}
