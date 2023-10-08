using UnityEngine;
using UnityEngine.EventSystems;

namespace SavedSettings.GUI
{
    /// <summary>
    /// Attach to a gameobject with an event system.
    /// If the currently selected gameobject for this event system is null, reselects the lasts selected game object.
    /// This serves as a simple fix for a control setup which both allows for a mouse and controller, in which a mouse could deselect everything on the menu.
    /// </summary>
    [RequireComponent(typeof(EventSystem))]
    public class ReSelect : MonoBehaviour
    {
        EventSystem _EventSystem;
        GameObject _Selected;

        void Start()
        {
            _EventSystem = GetComponent<EventSystem>();
            _Selected = _EventSystem.firstSelectedGameObject;
        }

        void Update()
        {
            if (_EventSystem.currentSelectedGameObject == null)
            {
                if (_Selected != null)
                {
                    _EventSystem.SetSelectedGameObject(_Selected);
                }
            }
            else if (_Selected != _EventSystem.currentSelectedGameObject)
            {
                _Selected = _EventSystem.currentSelectedGameObject;
            }
        }
    }
}