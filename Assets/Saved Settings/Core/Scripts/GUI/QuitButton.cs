using UnityEngine;
using UnityEngine.UI;

namespace SavedSettings.GUI
{
    /// <summary>
    /// Quits the application when the button is pressed.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class QuitButton : MonoBehaviour
    {
        void Start()
        {
            GetComponent<Button>().onClick.AddListener(delegate { Application.Quit(); });
        }
    }
}
