using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI; // 引用 Unity UI 命名空间
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;

    public Button ResumeButton; // 在 Inspector 中分配这些按钮
    public Button MenuButton;
    public Button QuitButton;

    private Button[] buttons;
    private int selectedIndex = -1;

    // 在 Start 方法中初始化按钮数组
    private void Start()
    {
        buttons = new Button[] { ResumeButton, MenuButton, QuitButton };
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("Escape key detected. Current pause state: " + GameIsPaused);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        // 如果游戏暂停，监听键盘输入以更新按钮选择
        if (GameIsPaused)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow)) // 下移
            {
                if (selectedIndex < buttons.Length - 1)
                    selectedIndex++;
                else
                    selectedIndex = 0;

                UpdateButtonSelection();
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow)) // 上移
            {
                if (selectedIndex > 0)
                    selectedIndex--;
                else
                    selectedIndex = buttons.Length - 1;

                UpdateButtonSelection();
            }
            else if (Input.GetKeyDown(KeyCode.Return)) // 激活按钮
            {
                if (selectedIndex > -1)
                    buttons[selectedIndex].onClick.Invoke();
            }
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameIsPaused = false;  // Reset the game state when a new scene is loaded
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        // IconManager.ShowKeyBinding();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
        GameIsPaused = true;
        selectedIndex = 0;
        UpdateButtonSelection();
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.UnloadSceneAsync("GameScene");
        SceneManager.LoadScene("StartScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    void UpdateButtonSelection()
    {
    Color selectedColor = Color.yellow; // 选中时的颜色
    Color normalColor = Color.white; // 未选中时的颜色

        for (int i = 0; i < buttons.Length; i++)
        {
            TextMeshProUGUI buttonText = buttons[i].GetComponentInChildren<TextMeshProUGUI>(); // 假设按钮的文本是它的子对象
            if (buttonText != null) // 确保找到了 Text 组件
            {
                buttonText.color = (i == selectedIndex) ? selectedColor : normalColor;
            }
            else
            {
                Debug.LogError("No Text component found for button at index " + i);
            }
        }
    }
}
