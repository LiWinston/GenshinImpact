using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EffectTimeBarUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    private float effectDuration;
    private float timer;
    private string effectType;

    public event Action<string> OnEffectBarExpired;
    public string EffectType { get { return effectType; } }

    public void Initialize(string effectType, Color fillColor, float duration)
    {
        this.effectType = effectType;
        fillImage.color = fillColor;
        effectDuration = duration;
        timer = 0f;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        fillImage.fillAmount = 1 - Mathf.Clamp01(timer / effectDuration);

        if (timer >= effectDuration)
        {
            // 触发事件通知NegativeEffectManager
            OnEffectBarExpired?.Invoke(effectType);
            // Destroy(gameObject);
        }
    }
}