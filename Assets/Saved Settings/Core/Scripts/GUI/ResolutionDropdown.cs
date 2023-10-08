using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SavedSettings.GUI
{
    /// <summary>
    /// Syncs the scroll rect with the resolution setting.
    /// </summary>
    [RequireComponent(typeof(ScrollRect))]
    [RequireComponent(typeof(AutoScrollDropDown))]
    public class ResolutionDropdown : BaseUILoadSetting
    {
        void Start()
        {
            Dropdown dropdown = GetComponent<Dropdown>();
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            Resolution[] resolutions = Screen.resolutions;
            Resolution res = Screen.currentResolution;
            int index = 0;
            int atIndex = 0;
            string str;

            for (int i = 0; i < resolutions.Length; ++i)
            {
                str = resolutions[i].ToString();
                atIndex = str.IndexOf('@');
                str = str.Remove(atIndex - 1);
                if (resolutions[i].width == res.width && resolutions[i].height == res.height)
                {
                    index = i;
                }
                options.Add(new Dropdown.OptionData(str));
            }
            dropdown.options = options;
            dropdown.value = index;


            dropdown.onValueChanged.AddListener(delegate
            {
                Screen.SetResolution(resolutions[dropdown.value].width, resolutions[dropdown.value].height, Screen.fullScreen);
            });
        }

        public override void LoadValue()
        {
            Dropdown dropdown = GetComponent<Dropdown>();
            Resolution[] resolutions = Screen.resolutions;
            Resolution res = Screen.currentResolution;
            int index = -1;
            for (int i = 0; i < resolutions.Length; ++i)
            {
                if (resolutions[i].width == res.width && resolutions[i].height == res.height)
                {
                    index = i;
                    break;
                }
            }
            if (index >= 0)
            {
                dropdown.value = index;
            }
        }
    }
}