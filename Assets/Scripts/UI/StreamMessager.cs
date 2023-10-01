using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class StreamMessager : MonoBehaviour
    {
        public Text messageText;
        public float displayDuration = 0.2f;
        public float fadeDuration = 0.1f;

        private Queue<string> messageQueue = new Queue<string>();
        private bool isDisplayingMessage = false;
        private float lastMessageTime = 0f; // 记录上一条消息的显示时间

        private void Start()
        {
            messageText.enabled = false;
        }

        private void Update()
        {
            if (!isDisplayingMessage && messageQueue.Count > 0)
            {
                string nextMessage = messageQueue.Dequeue();
                StartCoroutine(DisplayMessage(nextMessage));
                lastMessageTime = Time.time;
            }
            else if (isDisplayingMessage && Time.time - lastMessageTime >= 1f)
            {
                // 如果当前有消息正在显示且超过1秒没有新消息，则淡出当前消息
                StartCoroutine(FadeOutMessage());
                lastMessageTime = Time.time;
            }
        }

        public void ShowMessage(string message, bool isFading = true)
        {
            if (!isFading)
            {
                // 若 isFading 为 false，直接显示消息
                messageText.text = message;
                messageText.enabled = true;
                lastMessageTime = Time.time;
                StartCoroutine(FadeOutMessage());
            }
            else
            {
                if (isDisplayingMessage)
                {
                    // 如果当前有消息正在显示，覆盖上一条消息
                    StopAllCoroutines(); // 停止当前正在显示的消息的淡入淡出过程
                    StartCoroutine(FadeOutMessage());
                    lastMessageTime = Time.time;
                }
                messageQueue.Enqueue(message);
            }
        }

        private IEnumerator DisplayMessage(string message)
        {
            isDisplayingMessage = true;
            messageText.text = message;
            messageText.enabled = true;

            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
                messageText.color = new Color(messageText.color.r, messageText.color.g, messageText.color.b, alpha);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(displayDuration);

            elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
                messageText.color = new Color(messageText.color.r, messageText.color.g, messageText.color.b, alpha);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            messageText.enabled = false;
            messageText.color = new Color(messageText.color.r, messageText.color.g, messageText.color.b, 1f);

            isDisplayingMessage = false;
        }

        private IEnumerator FadeOutMessage()
        {
            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
                messageText.color = new Color(messageText.color.r, messageText.color.g, messageText.color.b, alpha);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            messageText.enabled = false;
            messageText.color = new Color(messageText.color.r, messageText.color.g, messageText.color.b, 1f);

            isDisplayingMessage = false;
        }
    }
}
