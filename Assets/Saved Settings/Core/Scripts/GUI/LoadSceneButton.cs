using UnityEngine;
using UnityEngine.UI;

namespace SavedSettings.GUI
{
    /// <summary>
    /// Syncs the button to changing a player's key bindings.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class LoadSceneButton : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField] int _SceneToLoad;
#pragma warning restore 649

        void Start()
        {
            GetComponent<Button>().onClick.AddListener(delegate { SceneHelper.LoadScene(0); });
        }
    }
}
