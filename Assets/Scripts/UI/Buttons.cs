using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class Buttons : MonoBehaviour
    {
        public void Start(){
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public void PlayGame()
        {
            SceneManager.LoadScene("GameScene");
        }
        public void QuitGame0()
        {
            Application.Quit();
        }

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
