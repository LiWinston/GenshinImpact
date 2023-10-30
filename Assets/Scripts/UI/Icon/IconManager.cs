using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using UnityEngine.SceneManagement;
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


        public static IconManager Instance
        {
            get
            { if (_instance != null) return _instance;
              _instance = GameObject.Find("IconManagerObject").AddComponent<IconManager>();
              _instance.InitializeIcons();

              return _instance; }
        }

        private void Awake()
        {
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void Start()
        {
            _instance = this;
            StartCoroutine(StartBehavior());
            InitializeIcons();
        }

        private IEnumerator StartBehavior()
        {
            yield return new WaitForSeconds(0.5f);
            foreach (var pair in _iconDictionary)
            {
                pair.Value.ShowOff();
            }
            yield return new WaitForSeconds(1f);
            UIManager.Instance.ShowMessage1("Press Tab to show key bindings");
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

        public void ShowIcon(string iconName)
        {
            IconName enumName;
            if (Enum.TryParse(iconName, true, out enumName) && _iconDictionary.ContainsKey(enumName))
            {
                _iconDictionary[enumName].ShowOn();
            }
            else
            {
                Debug.LogWarning("Icon with name " + iconName + " not found.");
            }
        }

        public void HideIcon(IconName iconName)
        {
            if (_iconDictionary.ContainsKey(iconName))
            {
                _iconDictionary[iconName].ShowOff();
            }
        }

        public void HideIcon(string iconName)
        {
            IconName enumName;
            if (Enum.TryParse(iconName, true, out enumName) && _iconDictionary.ContainsKey(enumName))
            {
                _iconDictionary[enumName].ShowOff();
            }
            else
            {
                Debug.LogWarning("Icon with name " + iconName + " not found.");
            }
        }

        
        public void InitIconWithKeyBinding(IconName iconName, KeyCode keyBinding)
        {
            if (_iconDictionary.TryGetValue(iconName, out var value))
            {
                value.KeyBinding = keyBinding;
            }
        }
        
        public void InitIconWithKeyBinding(string iconName, KeyCode key, bool isElapsing = false)
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
                    if (isElapsing)
                    {
                        value.IsElapsing = true;
                    }
                }
            }
        }

        
        
        public static void ShowKeyBinding(float time = 2.0f)
        {
            if (_instance == null)
            {
                // Handle the case when _instance is null, e.g., log an error or do nothing
                return;
            }
            for(int i = 0; i < Enum.GetValues(typeof(IconName)).Length; i++)
            {
                _iconDictionary[(IconName)i].ShowKeyBinding(time);
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
        
        private void OnSceneUnloaded(Scene scene)
        {
            // Debug.Log("ClearInstance !!!");
            _iconDictionary.Clear();
            _instance = null;
        }

        private void OnDestroy()
        {
            // Debug.Log("ClearInstanceManaul !!!");
            _iconDictionary.Clear();
            _instance = null;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }
        
    }
}