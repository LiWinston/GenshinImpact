using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using Object = UnityEngine.Object;

namespace UI
{
    public class IconManager : MonoBehaviour
    {
        private IconManager _instance;

        public enum IconName
        {
            Autophagy,
            ExtremeCold,
            MeadowMeteor,
            PersistentReverie,
            Fire,
            HurricaneKick,
            GoldenBell,
            ZenMode
        }
        private Dictionary<IconName, IIconControllable> _iconDictionary = new Dictionary<IconName, IIconControllable>();
        public Dictionary<IconName, IIconControllable> IconDictionary
        {
            get => _iconDictionary;
        }


        public IconManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.Find("IconManagerObject").AddComponent<IconManager>();
                    _instance.InitializeIcons();
                }

                return _instance;
            }
            set { }
        }

        private void Awake()
        {
            _instance = this;
            InitializeIcons();
        }


        private void InitializeIcons()
        {
            Dictionary<string, IconName> nameToIconMap = new Dictionary<string, IconName>();

            foreach (Transform child in transform)
            {
                string childName = child.name.ToLower();

                foreach (IconName iconName in Enum.GetValues(typeof(IconName)))
                {
                    string enumName = iconName.ToString().ToLower();

                    if (string.Equals(childName, enumName))
                    {
                        // Check if the name has already been encountered
                        if (nameToIconMap.ContainsKey(childName))
                        {
                            // Duplicate name found, raise an error and stop the game
                            Debug.LogError("Multiple icons with the same name found: " + childName);
#if UNITY_EDITOR
                            UnityEditor.EditorApplication.isPlaying = false;
#else
                            Application.Quit();
#endif
                            return;
                        }

                        nameToIconMap[childName] = iconName;

                        IIconControllable iconControllable = child.GetComponent<IIconControllable>();
                        if (iconControllable != null)
                        {
                            _iconDictionary.Add(iconName, iconControllable);
                        }
                    }
                }
            }
        }

        
    }
}