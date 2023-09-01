using System;
using UnityEngine;

namespace AttributeRelatedScript
{
    public class HealItemEffect : ItemEffectStrategyBase
    {
        public HealItemEffect(PlayerBuffEffect.EffectType et, float healAmount) : base(et, healAmount)
        {
        }

        public override void ApplyEffect(GameObject player)
        {
            // 在这里实现恢复生命值的逻辑
            player.GetComponent<Health>().Heal(effectValue); // 使用字段来指定恢复的生命值数量
            float currentHealth = player.GetComponent<Health>().CurrentHealth; // 获取玩家当前的生命值
            ShowEffectMessage(effectValue, currentHealth);

            // 调用内部类处理特效
            ParticleEffectHandler.HandleParticleEffect(player.transform.position);
        }

        // 内部类用于处理特效
        private class ParticleEffectHandler : MonoBehaviour
        {
            public static void HandleParticleEffect(Vector3 position)
            {
                // 添加特效
                GameObject particlePrefab = Resources.Load<GameObject>("Spiral_02.1");
                if (particlePrefab != null)
                {
                    GameObject particleEffect = Instantiate(particlePrefab, position, Quaternion.identity);
                    // 设置特效的持续时间
                    Destroy(particleEffect, 1f); // 持续一秒
                    // 添加淡入淡出效果
                    var particleRenderer = particleEffect.GetComponent<ParticleSystemRenderer>();
                    if (particleRenderer != null)
                    {
                        particleRenderer.material.color = new Color(1f, 1f, 1f, 0f); // 初始透明

                        // 使用协程实现淡入淡出效果
                        MonoBehaviour monoBehaviour = particleEffect.AddComponent<MonoBehaviour>();
                        monoBehaviour.StartCoroutine(FadeInAndOut(particleRenderer, particleEffect));
                    }
                }
            }

            private static System.Collections.IEnumerator FadeInAndOut(ParticleSystemRenderer renderer, GameObject effectObject)
            {
                float duration = 0.5f;
                float startTime = Time.time;
                Color startColor = renderer.material.color;
                Color targetColor = new Color(1f, 1f, 1f, 1f); // 目标不透明

                while (Time.time < startTime + duration)
                {
                    float t = (Time.time - startTime) / duration;
                    renderer.material.color = Color.Lerp(startColor, targetColor, t);
                    yield return null;
                }

                // 淡出
                startTime = Time.time;
                targetColor = new Color(1f, 1f, 1f, 0f); // 目标透明

                while (Time.time < startTime + duration)
                {
                    float t = (Time.time - startTime) / duration;
                    renderer.material.color = Color.Lerp(startColor, targetColor, t);
                    yield return null;
                }

                Destroy(effectObject); // 完成后销毁特效对象
            }
        }
    }
}
