using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SavedSettings.GUI
{
    /// <summary>
    /// Syncs the drop down with the anti-aliasing setting.
    /// </summary>
    [RequireComponent(typeof(Dropdown))]
    public class AntiAliasingDropdown : BaseUILoadSetting
    {
        void Start()
        {
            Dropdown dropdown = GetComponent<Dropdown>();
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            options.Add(new Dropdown.OptionData("Disabled"));
            options.Add(new Dropdown.OptionData("2x Multi Sampling"));
            options.Add(new Dropdown.OptionData("4x Multi Sampling"));
            options.Add(new Dropdown.OptionData("8x Multi Sampling"));

            dropdown.options = options;
            LoadValue();

            dropdown.onValueChanged.AddListener(delegate
            {

                switch (dropdown.value)
                {
                    case 1:
                        QualitySettings.antiAliasing = 2;
                        break;
                    case 2:
                        QualitySettings.antiAliasing = 4;
                        break;
                    case 3:
                        QualitySettings.antiAliasing = 8;
                        break;
                    default:
                        QualitySettings.antiAliasing = 0;
                        break;
                }
            });
        }

        /// <summary>
        /// Called by ResetSettings and DiscardSettings after the settings are changed.
        /// </summary>
        public override void LoadValue()
        {
            Dropdown dropdown = GetComponent<Dropdown>();
            switch (QualitySettings.antiAliasing)
            {
                case 2:
                    dropdown.value = 1;
                    break;
                case 4:
                    dropdown.value = 2;
                    break;
                case 8:
                    dropdown.value = 3;
                    break;
                default:
                    dropdown.value = 0;
                    break;
            }
        }
    }
}