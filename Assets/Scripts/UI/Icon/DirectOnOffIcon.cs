using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class DirectOnOffIcon : MonoBehaviour, IIconControllable
    {
        Image image;
        // Color originalColor; // 保存初始颜色
        [SerializeField]bool isElapsing = false; // 是否随即消失
        [SerializeField]float fadeDuration = 0.75f; // 调整这个值以控制渐变的速度
        [SerializeField] KeyCode keyBinding;
        private Text keyTextPrefab; // UI Text预制体
        private Text keyTextObject;
        [SerializeField] Transform canvasTransform; // Canvas的Transform

        private void Start()
        {
            image = GetComponent<Image>();
            // originalColor = image.color; // 保存初始颜色
            keyTextPrefab = Resources.Load<Text>("Prefab/UI/UITextPrefab");
        }

        public void ShowOn()
        {
            switch (IsElapsing)
            {
                case true:
                    StartCoroutine(FadeToAlpha(1.0f));
                    break;
                case false:
                    StartCoroutine(FadeToAlpha(1.0f, () =>
                    {
                        // 在协程完成后调用ShowOff
                        ShowOff();
                    }));
                    break;
            }
        }

        public void ShowOff()
        {
            // 淡出
            StartCoroutine(FadeToAlpha(0.4f));
        }

        public void ShowKeyBinding(float time)
        {
            StartCoroutine(ShowKeyBindingCoroutine(time));
        }

        public KeyCode KeyBinding
        {
            get => keyBinding;
            set => keyBinding = value;
        }

        public bool IsElapsing
        {
            get => isElapsing;
            set => isElapsing = value;
        }

        private IEnumerator ShowKeyBindingCoroutine(float time)
        {
            // 检查是否已存在 UI Text 对象
            if (keyTextObject == null)
            {
                // 创建UI Text对象
                keyTextObject = Instantiate(keyTextPrefab, transform);

                // 设置UI Text的本地偏移位置
                Vector2 localOffset = new Vector2(0f, -50f); // 根据需要调整本地偏移位置
                keyTextObject.transform.localPosition = localOffset;

                // 将KeyCode转换为对应的按键名称

                string keyName = keyBinding switch
                    {
                        KeyCode.Mouse0 => "L Click",
                        KeyCode.Mouse1 => "R Click",
                        KeyCode.UpArrow => "Up",
                        KeyCode.DownArrow => "Down",
                        KeyCode.LeftArrow => "Left",
                        KeyCode.RightArrow => "Right",
                        KeyCode.Space => "Space",
                        KeyCode.W => "W",
                        KeyCode.A => "A",
                        KeyCode.S => "S",
                        KeyCode.D => "D",
                        KeyCode.LeftControl => "L Ctrl",
                        KeyCode.LeftShift => "L Shift",
                        // 添加其他常见按键的映射
                        _ => keyBinding.ToString()
                    };

                // 设置UI Text的内容
                Text keyText = keyTextObject.GetComponent<Text>();
                keyText.text = keyName;
            }

            // 启用 UI Text 对象
            keyTextObject.enabled = true;

            // 等待一段时间后禁用UI Text对象
            yield return new WaitForSeconds(time); // 调整等待时间
            keyTextObject.enabled = false;
        }

        private IEnumerator FadeToAlpha(float targetAlpha, Action onComplete = null)
        {
            float elapsedTime = 0;
            Color currentColor = image.color;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.fixedDeltaTime;
                float t = Mathf.Clamp01(elapsedTime / fadeDuration);
                currentColor.a = Mathf.Lerp(currentColor.a, targetAlpha, t);
                image.color = currentColor;
                yield return null;
            }

            // 确保最终颜色准确设置为目标透明度
            currentColor.a = targetAlpha;
            // image.color = currentColor;
            onComplete?.Invoke();
        }
    }
}
