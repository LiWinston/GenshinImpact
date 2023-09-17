using System;
using System.Collections;
using UnityEngine;

public class ParticleEffectManager : MonoBehaviour
{
    public static ParticleEffectManager Instance;

    public GameObject particlePrefab; // 可以在编辑器中指定粒子特效预制体

    [Header("Settings")]
    public float defaultDuration = 1.5f;
    public bool autoDestroy = true; // 控制特效是否自动销毁
    private GameObject currentParticleEffect;

    private ObjectPooler objectPooler; // 如果使用对象池，需要一个对象池管理器

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        // 订阅退出禅模式事件，当事件触发时执行 StopParticleEffect 方法
        State.OnExitZenMode += StopParticleEffect;
    }

    public void PlayParticleEffect(GameObject player, Quaternion rotation, Color startColor,
        Color endColor, float duration = -1f)
    {
        Debug.LogWarning("using default Particle.");
        if (particlePrefab == null)
        {
            Debug.LogWarning("No default Particle.");
        }
        PlayParticleEffect(particlePrefab,  player,  rotation,  startColor, endColor,  duration);
    }

    public void PlayParticleEffect(string particlePrefabFile, GameObject player, Quaternion rotation, Color startColor,
        Color endColor, float duration = -1f)
    {
        var myParticlePrefab = Resources.Load<GameObject>(particlePrefabFile);
        PlayParticleEffect(myParticlePrefab, player, rotation, startColor, endColor, duration);
    }
    // ReSharper disable Unity.PerformanceAnalysis
    public void PlayParticleEffect(GameObject particlePrefab, GameObject player, Quaternion rotation, Color startColor, Color endColor, float duration = -1f)
    {
        var particleEffect = Instantiate(particlePrefab, player.transform.position, rotation);

        // 设置特效的持续时间
        if (duration < 0f)
        {
            duration = defaultDuration;
        }

        if (autoDestroy)
        {
            Destroy(particleEffect, duration);
        }

        // 设置特效的初始颜色
        var particleRenderer = particleEffect.GetComponent<ParticleSystemRenderer>();
        if (particleRenderer != null)
        {
            particleRenderer.material.color = startColor;
        }
        particleEffect.transform.SetParent(player.transform);
        // 添加淡入淡出效果
        StartCoroutine(FadeInAndOut(particleRenderer, particleEffect, duration, startColor, endColor));
    }

    private System.Collections.IEnumerator FadeInAndOut(ParticleSystemRenderer renderer, GameObject effectObject, float duration, Color startColor, Color endColor, Action onEffectEnd = null)
    {
        float startTime = Time.time;

        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            // renderer.material.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        // 淡出
        if (autoDestroy)
        {
            // objectPooler.ReturnToPool("ParticleEffects",effectObject);
            Destroy(effectObject);
        }

        // 调用特效结束回调
        onEffectEnd?.Invoke();
    }
    private IEnumerator PlayParticleEffectUntilEndCoroutine(GameObject particlePrefab, GameObject player, Quaternion rotation, Color startColor, Color endColor, Action onEffectEnd)
    {
        var particleEffect = Instantiate(particlePrefab, player.transform.position, rotation);

        // 设置特效的初始颜色
        var particleRenderer = particleEffect.GetComponent<ParticleSystemRenderer>();
        if (particleRenderer != null)
        {
            particleRenderer.material.color = startColor;
        }
        particleEffect.transform.SetParent(player.transform);

        // 添加淡入淡出效果
        float duration = -1f; // 播放直到显式结束
        float startTime = Time.time;

        while (duration < 0f || Time.time < startTime + duration)
        {
            float t = duration < 0f ? 0f : (Time.time - startTime) / duration;
            particleRenderer.material.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        // 手动结束特效时调用回调
        onEffectEnd?.Invoke();

        // 销毁特效
        Destroy(particleEffect);
    }

    public IEnumerator PlayParticleEffectUntilEndCoroutine(string particlePrefabFile, GameObject player, 
        Quaternion rotation, Color startColor, Color endColor, Action onEffectEnd)
    {
        var myParticlePrefab = Resources.Load<GameObject>(particlePrefabFile);
        currentParticleEffect = Instantiate(myParticlePrefab, player.transform.position, rotation);

        // 设置特效的初始颜色
        var particleRenderer = currentParticleEffect.GetComponent<ParticleSystemRenderer>();
        if (particleRenderer != null)
        {
            particleRenderer.material.color = startColor;
        }
        currentParticleEffect.transform.SetParent(player.transform);

        // 添加淡入淡出效果
        float duration = -1f; // 播放直到显式结束
        float startTime = Time.time;

        while (duration < 0f || Time.time < startTime + duration)
        {
            float t = duration < 0f ? 0f : (Time.time - startTime) / duration;
            particleRenderer.material.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        // 手动结束特效时调用回调
        onEffectEnd?.Invoke();

        // 销毁特效
        Destroy(currentParticleEffect);
        currentParticleEffect = null; // 清空引用
    }


    private void StopParticleEffect()
    {
        if (currentParticleEffect != null)
        {
            Destroy(currentParticleEffect);
            currentParticleEffect = null; // 清空引用
        }
    }
}
