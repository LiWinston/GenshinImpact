using UnityEngine;
using UnityEngine.UI;

namespace SavedSettings.GUI
{
    /// <summary>
    /// Syncs the button to discarding the changed key bindings.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class DiscardKeysButton : MonoBehaviour
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
                Debug.LogError("DiscardKeysButton needs an instance of PlayerKeyBindings set through the inspector.");
#endif
                return;
            }

            GetComponent<Button>().onClick.AddListener(delegate
            {
                _PlayerBindings.Load();
                foreach (BaseUILoadKey key in _LoadKeys)
                {
                    if (key != null)
                    {
                        key.LoadValue();
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