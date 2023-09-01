using System;
using UnityEngine;

public class ParticleEffectManager : MonoBehaviour
{
    public static ParticleEffectManager Instance;

    public GameObject particlePrefab; // 可以在编辑器中指定粒子特效预制体

    [Header("Settings")]
    public float defaultDuration = 1.5f;
    public bool autoDestroy = true; // 控制特效是否自动销毁
    public bool usePooling = true; // 使用对象池来管理特效

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

        if (usePooling)
        {
            objectPooler = GetComponent<ObjectPooler>();
            if (objectPooler == null)
            {
                Debug.LogWarning("Object Pooler component not found on ParticleEffectManager. Disabling pooling.");
                usePooling = false;
            }
        }
    }

    public void PlayParticleEffect(Vector3 position, Quaternion rotation, Color startColor,
        Color endColor, float duration = -1f)
    {
        Debug.LogWarning("using default Particle.");
        if (particlePrefab == null)
        {
            Debug.LogWarning("No default Particle.");
        }
        PlayParticleEffect(particlePrefab,  position,  rotation,  startColor, endColor,  duration);
    }

    public void PlayParticleEffect(String particlePrefabFile, Vector3 position, Quaternion rotation, Color startColor,
        Color endColor, float duration = -1f)
    {
        GameObject particlePrefab = Resources.Load<GameObject>(particlePrefabFile);
        PlayParticleEffect(particlePrefab, position, rotation, startColor, endColor, duration);
    }
    public void PlayParticleEffect(GameObject particlePrefab, Vector3 position, Quaternion rotation, Color startColor, Color endColor, float duration = -1f)
    {
        

        GameObject particleEffect;

        if (usePooling)
        {
            // 从对象池中获取特效对象
            particleEffect = objectPooler.SpawnFromPool(particlePrefab.name, position, rotation);
        }
        else
        {
            // 直接实例化特效对象
            particleEffect = Instantiate(particlePrefab, position, rotation);
        }

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

        // 添加淡入淡出效果
        StartCoroutine(FadeInAndOut(particleRenderer, particleEffect, duration, startColor, endColor));
    }

    private System.Collections.IEnumerator FadeInAndOut(ParticleSystemRenderer renderer, GameObject effectObject, float duration, Color startColor, Color endColor)
    {
        float startTime = Time.time;

        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            renderer.material.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        // 淡出
        if (autoDestroy)
        {
            if (usePooling)
            {
                objectPooler.ReturnToPool("ParticleEffects",effectObject);
            }
            else
            {
                Destroy(effectObject);
            }
        }
    }
}
