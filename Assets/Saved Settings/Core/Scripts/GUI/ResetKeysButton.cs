using UnityEngine;
using UnityEngine.UI;

namespace SavedSettings.GUI
{
    /// <summary>
    /// When pressed, the button will reset the given key bindings.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class ResetKeysButton : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField] PlayerKeyBindings _PlayerBindings;
        [SerializeField] BaseUILoadKey[] _LoadKeys;
#pragma warning restore 649

        void Start()
        {
            if (_PlayerBindings == null)
            {
#if UNITY_EDITOR
                Debug.LogError("ResetKeysButton needs an instance of PlayerKeyBindings set through the inspector.");
#endif
                return;
            }

            GetComponent<Button>().onClick.AddListener(delegate
            {
                _PlayerBindings.ResetKeyBindings();
                foreach (BaseUILoadKey loadKey in _LoadKeys)
                {
                    if (loadKey != null)
                    {
                        loadKey.LoadValue();
                    }
                }
            });
        }

#if UNITY_EDITOR

        private void Reset()
        {
            _PlayerBindings = FindObjectOfType<PlayerKeyBindings>();
            _LoadKeys = FindObjectsOfType<BaseUILoadKey>();
        }

#endif
    }
}

