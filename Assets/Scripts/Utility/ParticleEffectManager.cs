using System;
using System.Collections;
using AttributeRelatedScript;
using UI;
using UnityEngine;

namespace Utility
{
    public class ParticleEffectManager : MonoBehaviour
    {
        public static ParticleEffectManager Instance;//单例

        public GameObject particlePrefab; // 默认特效预制体

        [Header("Settings")]
        public float defaultDuration = 1.5f;
        public bool autoDestroy = true; // 控制特效是否自动销毁
        private GameObject currentParticleEffect;

    

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

        public void PlayParticleEffect(string particlePrefabFile, GameObject player, Quaternion rotation, Color startColor ,
            Color endColor, float duration = -1f)
        {
            var myParticlePrefab = Resources.Load<GameObject>(particlePrefabFile);
            PlayParticleEffect(myParticlePrefab, player, rotation, startColor, endColor, duration);
        }

        public void PlayParticleEffect(GameObject particlePrefab, GameObject player, Quaternion rotation, Color startColor, Color endColor, float duration = -1f)
        {
            var particleEffect = Instantiate(particlePrefab, player.transform.position, rotation);
            var particleSystemComponent = particleEffect.GetComponent<ParticleSystem>();

            // 设置特效的持续时间
            if (duration < 0f)
            {
                // duration = defaultDuration;
                duration = particleSystemComponent.totalTime;
            }

            if (autoDestroy)
            {
                Destroy(particleEffect, duration);
            }

            // 设置特效的初始颜色
            var particleMain = particleSystemComponent.main;
            particleMain.startColor = startColor;

            particleEffect.transform.SetParent(player.transform);

            // 添加淡入淡出效果
            StartCoroutine(FadeInAndOut(particleSystemComponent, particleEffect, duration, startColor, endColor));
        }

        private IEnumerator FadeInAndOut(ParticleSystem particleSystem, GameObject particleEffect, float duration, Color startColor, Color endColor)
        {
            //This lead to RT Error and have no obvious effect. Delete it.
        
            // float elapsedTime = 0f;
            //
            // while (elapsedTime < duration)
            // {
            //     // float lerpValue = elapsedTime / duration;
            //     // Color lerpedColor = Color.Lerp(startColor, endColor, lerpValue);
            //
            //     // 修改粒子系统的颜色
            //     // var main = particleSystem.main;
            //     // main.startColor = lerpedColor;
            //
            //     elapsedTime += Time.deltaTime;
            //     yield return null;
            // }

            // // 确保结束时颜色是endColor
            // var finalMain = particleSystem.main;
            // finalMain.startColor = endColor;

            // 在淡出后销毁特效
            yield return new WaitForSeconds(duration);
            if (autoDestroy)
            {
                Destroy(particleEffect);
            }
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
            Quaternion rotation, Color startColor, Color endColor , Action onEffectEnd = null)
        {
            if (currentParticleEffect) yield break;
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
        }


        private void StopParticleEffect()
        {
            // UIManager.Instance.ShowMessage2("StopParticleEffect()");
            if (currentParticleEffect != null)
            {
                Destroy(currentParticleEffect);
                currentParticleEffect = null; // 清空引用
            }
        }
    }
}
