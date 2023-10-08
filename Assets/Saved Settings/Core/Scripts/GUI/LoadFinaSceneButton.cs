using UnityEngine;
using UnityEngine.UI;

namespace SavedSettings.GUI
{
    /// <summary>
    /// Syncs the button to changing a player's key bindings.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class LoadFinaSceneButton : MonoBehaviour
    {
        void Start()
        {
            GetComponent<Button>().onClick.AddListener(delegate { SceneHelper.FinalScene(); });
        }
    }
}
