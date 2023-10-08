using System.Collections.Generic;
using UnityEngine;

namespace SavedSettings
{
    /// <summary>
    /// Contains a dictionary of keybindings that are loaded on startup.
    /// The default keybindings are changeable through the inspector.
    /// You can create a PlayerKeyBinding from Assets/Create/ScriptableObjects/KeyBindings.
    /// You can access these keybindings using (referenceToThePlayerKeyBindings).Keys["Right"]
    /// When the player finishes setting the keys be sure to call Save(), or call Load() to discard any changes.
    /// If you add/remove keybindings after release you have to update the version number.
    /// When data is saved, it’s saved with the game version number attached to the data.
    /// Please see scriptable objects unity documentation for a better idea as to how this works.
    /// </summary>
    [CreateAssetMenu(fileName = "KeyBindings", menuName = "ScriptableObjects/KeyBindings")]
    public class PlayerKeyBindings : ScriptableObject
    {
        public Dictionary<string, KeyBinding> Keys = new Dictionary<string, KeyBinding> { };
        const string PATH_KEY_BINDINGS = "KeyBindings_";

        // Set the bindings dictionary through an array that's visisble in the inspector.
#pragma warning disable 649
        [SerializeField] KeyBinding[] _DefaultBindings;
        [SerializeField] float version;
#pragma warning restore 649

        /// <summary>
        /// When the bindings are loaded in the scene, load bindings from a json file.
        /// </summary>
        private void OnEnable()
        {
            Load();
        }

        /// <summary>
        /// Loads the player key bindings from a saved json file.
        /// This also acts as a Discard() function.
        /// In the Unity editor these are always overwritten to the default keys.
        /// </summary>
        public virtual void Load()
        {
#if UNITY_EDITOR
            ResetKeyBindings();
#else
        SaveData data = SaveHelper.Load(PATH_KEY_BINDINGS + this.GetInstanceID());
        if (data.data != null && data.data != string.Empty)
        {
            /* If you have already released the game and must update the keybindings,
             * be sure to update the version number of the key binding through the inspector,
             * and add some handling for loading old versions of the the keybindings here (or in an inheriting class).
             */

            Keys = JsonDictionary.jsonToDictionary<string, KeyBinding>(data.data);
        }
        else
        {
            ResetKeyBindings ();
        }
#endif
        }

        /// <summary>
        /// Loads the player key bindings from a saved json file.
        /// </summary>
        public void Save()
        {
            SaveHelper.Save(PATH_KEY_BINDINGS + this.GetInstanceID(), JsonDictionary.dictionaryToJson(Keys), 1f);
        }

        /// <summary>
        /// Resets the key bindings to the defaults and saves.
        /// </summary>
        public void ResetKeyBindings()
        {
            Keys.Clear();
            for (int i = 0; i < _DefaultBindings.Length; ++i)
            {
                Keys.Add(_DefaultBindings[i].name, new KeyBinding(_DefaultBindings[i].name,
                                                                    _DefaultBindings[i].keyA,
                                                                    _DefaultBindings[i].keyB));
            }
            Save();
        }

        /// <summary>
        /// A key binding consists of 2 keycodes and helper functions to get their input.
        /// </summary>
        [System.Serializable]
        public struct KeyBinding
        {
            public string name;
            public KeyCode keyA;
            public KeyCode keyB;

            public KeyBinding(string aName, KeyCode a, KeyCode b)
            {
                name = aName;
                keyA = a;
                keyB = b;
            }

            /// <summary>
            /// If either key is held.
            /// </summary>
            public bool Held
            {
                get { return Input.GetKey(keyA) || Input.GetKey(keyB); }
            }

            /// <summary>
            /// If either key has just been pressed down.
            /// </summary>
            public bool Down
            {
                get { return Input.GetKeyDown(keyA) || Input.GetKeyDown(keyB); }
            }

            /// <summary>
            /// If either key has just stopped being held.
            /// </summary>
            public bool Up
            {
                get { return Input.GetKeyUp(keyA) || Input.GetKeyUp(keyB); }
            }

            /// <summary>
            /// Gets input on a scale between -1 to 1 using another keybinding as the negative.
            /// </summary>
            public float AxisInput(KeyBinding negative)
            {
                float axis = 0f;
                if (Held)
                {
                    axis += 1f;
                }
                if (negative.Held)
                {
                    axis -= 1f;
                }
                return axis;
            }
        }

#if UNITY_EDITOR

        /// <summary>
        /// Create 5 default key bindings.
        /// </summary>
        void Reset()
        {
            _DefaultBindings = new KeyBinding[5];
            _DefaultBindings[0] = new KeyBinding("Up", KeyCode.W, KeyCode.UpArrow);
            _DefaultBindings[1] = new KeyBinding("Down", KeyCode.S, KeyCode.DownArrow);
            _DefaultBindings[2] = new KeyBinding("Right", KeyCode.D, KeyCode.RightArrow);
            _DefaultBindings[3] = new KeyBinding("Left", KeyCode.A, KeyCode.LeftArrow);
            _DefaultBindings[4] = new KeyBinding("Jump", KeyCode.Space, KeyCode.Keypad0);
        }

#endif
    }
}