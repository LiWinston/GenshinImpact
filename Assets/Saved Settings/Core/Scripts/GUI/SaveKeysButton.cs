using UnityEngine;
using UnityEngine.UI;

namespace SavedSettings.GUI
{
    /// <summary>
    /// Syncs the button to saving the current key bindings.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class SaveKeysButton : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField] PlayerKeyBindings _PlayerBindings;
#pragma warning restore 649

        void Start()
        {
            if (_PlayerBindings == null)
            {
#if UNITY_EDITOR
                Debug.LogError("SaveKeysButton needs an instance of PlayerKeyBindings set through the inspector.");
#endif
                return;
            }

            GetComponent<Button>().onClick.AddListener(delegate
            {
                _PlayerBindings.Save();
            });
        }
    }
}
