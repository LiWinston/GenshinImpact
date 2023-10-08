using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SavedSettings.GUI
{
    /// <summary>
    /// Causes a dropdown list to auto-scroll to match keycode/joystick input.
    /// </summary>
    [RequireComponent(typeof(Dropdown))]
    public class AutoScrollDropDown : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        bool mouseOver;
        Dropdown dropdown;
        const string ITEM_NAME = "Item ";

        /// <summary>
        /// Get the dropdown to auto scroll
        /// </summary>
        void Awake()
        {
            dropdown = GetComponent<Dropdown>();
        }

        /// <summary>
        /// Don't use auto scroll if there are no options to scroll
        /// </summary>
        void OnEnable()
        {
            if (dropdown.options.Count == 0)
            {
                enabled = false;
            }
        }

        /// <summary>
        /// Check if the 
        /// </summary>
        void Update()
        {
            // If hovered over with a mouse do not use auto-scroll. Let the player use the scroll bar.
            if (mouseOver)
            {
                return;
            }

            // Autoscroll list as the selected object is changed from the arrow keys or a controller
            GameObject selected = EventSystem.current.currentSelectedGameObject;
            if (selected != null &&
                selected.transform.IsChildOf(transform) &&
                selected.name.StartsWith(ITEM_NAME))
            {
                Transform parent = selected.transform.parent;
                Image mask = parent.GetComponentInParent<Image>();
                Scrollbar scrollbar = GetComponentInChildren<Scrollbar>();
                if (parent == null || mask == null || scrollbar == null || !scrollbar.gameObject.activeInHierarchy)
                {
                    return;
                }

                // Get information used for clamping this drop down to the mask
                float itemHeight = selected.GetComponentInChildren<Image>().rectTransform.rect.height;
                float maskHeight = mask.rectTransform.rect.height;
                float itemPos = selected.transform.position.y + itemHeight / 2f;
                float yMaskPos = mask.rectTransform.position.y;
                float totalHeight = itemHeight * (parent.childCount - 1);

                // Above mask
                float dist = itemPos - yMaskPos;
                if (dist > 0f)
                {
                    scrollbar.value += dist / totalHeight;
                    return;
                }

                // Below mask
                dist = (yMaskPos - maskHeight) - (itemPos - itemHeight);
                if (dist > 0f)
                {
                    scrollbar.value -= dist / totalHeight;
                    return;
                }
            }
        }

        /// <summary>
        /// If hovered over with a mouse do not use auto-scroll. Let the player use the scroll bar.
        /// </summary>
        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            mouseOver = false;
        }
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            mouseOver = true;
        }
    }
}
