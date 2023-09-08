using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager instance; // 单例引用器
        private Messager UIMessage_1MSG;
        private Messager UIMessage_2MSG;
        
        // 获取单例实例的静态属性
        public static UIManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<UIManager>(); // 查找已存在的实例
                    if (instance == null)
                    {
                        // 如果没有现有实例，创建一个新的GameObject并附加UIManager组件
                        GameObject obj = new GameObject("UIManager");
                        instance = obj.AddComponent<UIManager>();
                    }
                }
                return instance;
            }
        }
        private static Messager FindUIMessage1()
        {
            Messager UIMessage_1MSG = GameObject.Find("UIMessage_1")?.GetComponent<Messager>();

            if (UIMessage_1MSG == null)
            {
                throw new System.Exception("UIMessage_1 not found or Message component missing!");
            }

            return UIMessage_1MSG;
        }
        private static Messager FindUIMessage2()
        {
            Messager UIMessage_2MSG = GameObject.Find("UIMessage_2")?.GetComponent<Messager>();

            if (UIMessage_2MSG == null)
            {
                throw new System.Exception("UIMessage_2 not found or Message component missing!");
            }

            return UIMessage_2MSG;
        }
        private void Awake()
        {
            // 确保只有一个实例存在，如果已经存在实例，则销毁新的实例
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject); // 保留实例在场景之间的切换中
            }
            else
            {
                Destroy(gameObject);
            }
        }
        private void Start()
        {
            UIMessage_1MSG = FindUIMessage1();
            UIMessage_2MSG = FindUIMessage2();
        }

        public void ShowMessage1(string message)
        {
            
            UIMessage_1MSG.ShowMessage(message);
        }
        
        
        public void ShowMessage2(string message)
        {
            
            UIMessage_2MSG.ShowMessage(message);
        }

        
    }
    
    
    
    public class Messager : MonoBehaviour
    {
        public Text messageText;
        public float displayDuration = 0.7f;
        public float fadeDuration = 0.3f;

        private Queue<string> messageQueue = new Queue<string>();
        private bool isDisplayingMessage = false;

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
            }
        }

        public void ShowMessage(string message)
        {
            messageQueue.Enqueue(message);
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
    }
}
