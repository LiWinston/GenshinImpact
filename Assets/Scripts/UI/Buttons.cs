using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class Buttons : MonoBehaviour
    {
        // Display the mouse and unlock it
        public void Start(){
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        // Start the game
        public void PlayGame()
        {
            SceneManager.LoadScene("GameScene");
        }

        // Quit the game
        public void QuitGame0()
        {
            Application.Quit();
        }

        // Restart the game
        public void RestartGame()
        {
            SceneManager.LoadScene("GameScene");
        }

        public void QuitGame()
        {
            SceneManager.LoadScene("StartScene");
        }
    }
}
