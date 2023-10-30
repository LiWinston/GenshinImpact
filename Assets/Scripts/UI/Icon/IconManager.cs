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
        private static IconManager _instance;

        public enum IconName
        {
            Autophagy,
            ExtremeCold,
            MeadowMeteor,
            PersistentReverie,
            Fire,
            HurricaneKick,
            GoldenBell,
            ZenMode,
            Sprint,
        }
        private static Dictionary<IconName, IIconControllable> _iconDictionary = new Dictionary<IconName, IIconControllable>();
        public static Dictionary<IconName, IIconControllable> IconDictionary
        {
            get => _iconDictionary;
        }


        public static IconManager Instance()
        {
            if (_instance != null) return _instance;
            _instance = GameObject.Find("IconManagerObject").AddComponent<IconManager>();
            _instance.InitializeIcons();

            return _instance;
        }

        private void Start()
        {
            _instance = this;
            InitializeIcons();
            foreach (var pair in _iconDictionary)
            {
                pair.Value.ShowOff();
            }
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                ShowKeyBinding();
            }
        }

        public void ShowIcon(IconName iconName)
        {
            if (_iconDictionary.ContainsKey(iconName))
            {
                _iconDictionary[iconName].ShowOn();
            }
        }
        
        public void HideIcon(IconName iconName)
        {
            if (_iconDictionary.ContainsKey(iconName))
            {
                _iconDictionary[iconName].ShowOff();
            }
        }
        
        public void SetKeyBinding(IconName iconName, KeyCode keyBinding)
        {
            if (_iconDictionary.TryGetValue(iconName, out var value))
            {
                value.KeyBinding = keyBinding;
            }
        }
        
        public void SetKeyBinding(string iconName, KeyCode key)
        {
            IconName enumName;
            if (Enum.TryParse(iconName, true,out enumName))
            {
                // if (_iconDictionary.ContainsKey(enumName))
                // {
                //     _iconDictionary[enumName].KeyBinding = key;
                // }
                //Simplify using C# feature
                if (_iconDictionary.TryGetValue(enumName, out var value))
                {
                    value.KeyBinding = key;
                }
            }
        }

        
        
        public static void ShowKeyBinding()
        {
            for(int i = 0; i < Enum.GetValues(typeof(IconName)).Length; i++)
            {
                _iconDictionary[(IconName)i].ShowKeyBinding();
            }
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