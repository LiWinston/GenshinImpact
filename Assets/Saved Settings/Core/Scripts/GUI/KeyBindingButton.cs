using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SavedSettings.GUI
{
    /// <summary>
    /// Syncs the button to changing a player's key bindings.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class KeyBindingButton : BaseUILoadKey
    {
#pragma warning disable 649
        [SerializeField] PlayerKeyBindings _PlayerBindings;
        [SerializeField] string BindingName;
        [SerializeField] bool leftBinding = true;
        [SerializeField] List<KeyCode> _KeysToIgnore = new List<KeyCode> { KeyCode.Escape, KeyCode.Return, KeyCode.KeypadEnter, KeyCode.Backspace, KeyCode.Menu };
        public string textWhileWaiting = "Hit Any Key";
#pragma warning restore 649
        Text _Text;
        bool _NoInput = true;

        void Start()
        {
            if (_PlayerBindings == null)
            {
#if UNITY_EDITOR
                Debug.LogError("KeyBindingButton needs an instance of PlayerKeyBindings set through the inspector.");
#endif
                return;
            }
            _Text = GetComponentInChildren<Text>();
            LoadValue();
            enabled = false;

            GetComponent<Button>().onClick.AddListener(delegate
            {
                if (_Text != null)
                {
                    _Text.text = textWhileWaiting;
                }
                _NoInput = true;
                enabled = true;
            });
        }

        // When disabled, reser the menu text
        private void OnDisable()
        {
            if (_Text != null)
            {
                if (leftBinding)
                {
                    _Text.text = _PlayerBindings.Keys[BindingName].keyA.ToString();
                }
                else
                {
                    _Text.text = _PlayerBindings.Keys[BindingName].keyB.ToString();
                }
            }
        }

        /// <summary>
        /// Get next key hit.
        /// </summary>
        private void Update()
        {
            if (_NoInput)
            {
                _NoInput = false;
                return;
            }

            if (Input.anyKeyDown)
            {
                foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(key))
                    {
                        // Ignore certain keys
                        // Typically you should not let the player set  keys that are used for the menu like ESC, ENTER, etc...
                        if (!_KeysToIgnore.Contains(key))
                        {
                            // Set the new key binding
                            if (leftBinding)
                            {
                                _PlayerBindings.Keys[BindingName] = new PlayerKeyBindings.KeyBinding(_PlayerBindings.Keys[BindingName].name,
                                                                                                        key,
                                                                                                        _PlayerBindings.Keys[BindingName].keyB);
                            }
                            else
                            {
                                _PlayerBindings.Keys[BindingName] = new PlayerKeyBindings.KeyBinding(_PlayerBindings.Keys[BindingName].name,
                                                                                                        _PlayerBindings.Keys[BindingName].keyA,
                                                                                                        key);
                            }

                            // Set the new button text
                            if (_Text != null)
                            {
                                _Text.text = key.ToString();
                            }

                            // Stop searching for input
                            enabled = false;
                        }
                        return;
                    }
                }
            }
        }

        public override void LoadValue()
        {
            if (_Text != null)
            {
                if (leftBinding)
                {
                    _Text.text = _PlayerBindings.Keys[BindingName].keyA.ToString();
                }
                else
                {
                    _Text.text = _PlayerBindings.Keys[BindingName].keyB.ToString();
                }
            }
        }
    }
}
