using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Messager : MonoBehaviour
    {
        public Text messageText;
        public float displayTime = 2.0f; // 显示时间，以秒为单位

        private float displayTimer;
        private bool isDisplayingMessage = false;

        private void Start()
        {
            messageText.enabled = false; // 初始时隐藏文本
        }

        private void Update()
        {
            if (isDisplayingMessage)
            {
                displayTimer -= Time.deltaTime;
                if (displayTimer <= 0)
                {
                    ClearMessage();
                }
            }
        }

        public void ShowMessage(string message)
        {
            messageText.text = message;
            messageText.enabled = true;
            isDisplayingMessage = true;
            displayTimer = displayTime;
        }

        private void ClearMessage()
        {
            messageText.text = "";
            messageText.enabled = false;
            isDisplayingMessage = false;
        }
    }
}