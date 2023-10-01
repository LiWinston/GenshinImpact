using AttributeRelatedScript;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UpdateLevelText : MonoBehaviour
{
    private Text levelText; // Reference to the Text component
    private State playerState; // Reference to the State script

    private void Start()
    {
        // Get the Text component
        levelText = GetComponent<Text>();

        // Find the State script containing level information
        playerState = FindObjectOfType<State>();

        // Check if Text component and State script are found
        if (levelText == null)
        {
            Debug.LogError("UpdateLevelText: Text component not found!");
        }

        if (playerState == null)
        {
            Debug.LogError("UpdateLevelText: State script not found!");
        }

        // Subscribe to the level change event
        if (playerState != null)
        {
            playerState.OnLevelChanged += HandleLevelChanged;
        }
    }

    private void HandleLevelChanged(int newLevel)
    {
        // Update the Text component's text with the new level
        if (levelText != null)
        {
            levelText.text = newLevel.ToString();
        }
        UIManager.Instance.ShowMessage2("LEVEL UP TO " + newLevel + "!");
    }

    private void OnDestroy()
    {
        // Unsubscribe from the level change event to prevent memory leaks
        if (playerState != null)
        {
            playerState.OnLevelChanged -= HandleLevelChanged;
        }
    }
}