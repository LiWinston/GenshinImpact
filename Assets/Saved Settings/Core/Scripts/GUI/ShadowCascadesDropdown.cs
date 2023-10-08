using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SavedSettings.GUI
{
    /// <summary>
    /// Syncs the drop down with the shadow cascade setting.
    /// </summary>
    [RequireComponent(typeof(Dropdown))]
    public class ShadowCascadesDropdown : BaseUILoadSetting
    {
        void Start()
        {
            Dropdown dropdown = GetComponent<Dropdown>();
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            options.Add(new Dropdown.OptionData("No Cascades"));
            options.Add(new Dropdown.OptionData("Two Cascades"));
            options.Add(new Dropdown.OptionData("Four Cascades"));
            dropdown.options = options;
            LoadValue();
            dropdown.onValueChanged.AddListener(delegate
            {

                switch (dropdown.value)
                {
                    case 1:
                        QualitySettings.shadowCascades = 2;
                        break;
                    case 2:
                        QualitySettings.shadowCascades = 4;
                        break;
                    default:
                        QualitySettings.shadowCascades = 0;
                        break;
                }
            });
        }

        public override void LoadValue()
        {
            Dropdown dropdown = GetComponent<Dropdown>();
            switch (QualitySettings.shadowCascades)
            {
                case 2:
                    dropdown.value = 1;
                    break;
                case 4:
                    dropdown.value = 2;
                    break;
                default:
                    dropdown.value = 0;
                    break;
            }
        }
    }
}
