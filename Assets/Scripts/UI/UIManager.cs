using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI
{
    public static class UIManager
    {
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

        public static void ShowMessage1(string message)
        {
            Messager UIMessage_1MSG = FindUIMessage1();
            UIMessage_1MSG.ShowMessage(message);
        }
        
        
        public static void ShowMessage2(string message)
        {
            Messager UIMessage_2MSG = FindUIMessage2();
            UIMessage_2MSG.ShowMessage(message);
        }

        public static void ShowExp(string expText)
        {
            // 查找场景内所有名称为 "ExpText" 的对象
            TextMeshPro[] expTextObjects = Resources.FindObjectsOfTypeAll<TextMeshPro>();
            // if(expTextObjects.Length == 0){ShowMessage1("No txterPro");}

            foreach (TextMeshPro textMesh in expTextObjects)
            {
                if (textMesh != null)
                {
                    textMesh.text = expText;
                    textMesh.alpha = 1f; // 设置初始透明度为1，完全可见
                    textMesh.gameObject.SetActive(true);

                    // 启动协程来淡出经验值显示
                    MonoBehaviour monoBehaviour = textMesh.gameObject.GetComponent<MonoBehaviour>();
                    monoBehaviour.StartCoroutine(FadeOutExpText(textMesh));
                }
                else
                {
                    Debug.LogError("TextMeshPro component not found on an ExpText GameObject.");
                }
            }
        }

        private static IEnumerator FadeOutExpText(TextMeshPro textMesh, float time = 0.8f, float fadeTime = 0.5f)
        {
            // 延迟一段时间以便观察经验值文本
            yield return new WaitForSeconds(time);

            float fadeDuration = fadeTime;
            float startAlpha = textMesh.alpha;
            float currentTime = 0f;

            while (currentTime < fadeDuration)
            {
                currentTime += Time.deltaTime;
                float newAlpha = Mathf.Lerp(startAlpha, 0f, currentTime / fadeDuration);
                textMesh.alpha = newAlpha;
                yield return null;
            }

            textMesh.gameObject.SetActive(false);
        }
    }
}