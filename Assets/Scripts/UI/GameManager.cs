using Behavior;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class GameManager : MonoBehaviour
    {
        public Text timerText;
        private float startTime;
        private bool gameEnded = false;
        public Transform bossRoom;
        private bool isFinalBattle = false;

        private void Start()
        {
            // 获取当前场景加载的时间
            startTime = 0;
        }

        private void Update()
        {
            if (!gameEnded)
            {
                // 计算从场景加载开始经过的时间
                float elapsedTime = Time.timeSinceLevelLoad - startTime;

                // 计算剩余时间
                float remainingTime = 240 - elapsedTime; // 300秒 = 5分钟

                // 更新倒计时文本
                UpdateTimerText(remainingTime);

                if (elapsedTime >= 4) // 240秒 = 4分钟
                {
                    if (!isFinalBattle)
                    {
                        // 触发决战事件，将玩家传送至指定位置
                        UIManager.Instance.UIMessage_2MSG.messageText.text = "";
                        UIManager.Instance.ShowMessage2("The decisive battle is coming! Hold on!");
                        TeleportPlayerToFloorLarge();
                    }
                    
                    if (elapsedTime >= 10) // 300秒 = 5分钟
                    {
                        // 游戏胜利，加载WinScene场景
                        LoadWinScene();
                    }
                }
            }
        }

        private void UpdateTimerText(float remainingTime)
        {
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);

            if (remainingTime >= 60)
            {
                timerText.text = "Countdown to the decisive battle: " + minutes.ToString("00") + " Min " + seconds.ToString("00") + " s";
            }
            else
            {
                timerText.text = "Hold On till 1 minute ends: " + seconds.ToString("00") + " s";
            }
        }

        private void TeleportPlayerToFloorLarge()
        {
            PlayerController.Instance.transform.position = bossRoom.position + Vector3.up * 2f;
            isFinalBattle = true;
        }

        private void LoadWinScene()
        {
            // 游戏胜利，加载WinScene场景
            SceneManager.LoadScene("WinScene");
            gameEnded = true;
        }
    }
}
