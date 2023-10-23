using System.Collections;
using System.Collections.Generic;
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
        SceneManager.LoadScene("StartScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    void UpdateButtonSelection()
    {
    Color selectedColor = Color.red; // 选中时的颜色
    Color normalColor = Color.white; // 未选中时的颜色

        for (int i = 0; i < buttons.Length; i++)
        {
        Text buttonText = buttons[i].GetComponentInChildren<Text>(); // 假设按钮的文本是它的子对象
            if (buttonText != null) // 确保找到了 Text 组件
            {
            buttonText.color = (i == selectedIndex) ? selectedColor : normalColor;
            }
        }
    }
}
