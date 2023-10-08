using UnityEngine;
using UnityEngine.UI;

namespace SavedSettings.GUI
{
    /// <summary>
    /// Toggles for a canvas to be enabled or disabled using a button.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class CanvasButton : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField] Canvas _Canvas;
#pragma warning restore 649

        void Start()
        {
            if (_Canvas)
            {
                GetComponent<Button>().onClick.AddListener(delegate { _Canvas.enabled = !_Canvas.enabled; });
            }
        }
    }
}
