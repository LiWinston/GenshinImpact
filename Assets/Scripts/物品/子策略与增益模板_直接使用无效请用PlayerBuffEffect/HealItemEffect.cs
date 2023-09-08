using System;
using Unity.Mathematics;
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
            player.GetComponent<State>().Heal(effectValue); // 使用字段来指定恢复的生命值数量
            float currentHealth = player.GetComponent<State>().CurrentHealth; // 获取玩家当前的生命值
            if (!player.GetComponent<State>().IsFullHealth())
            {
                ShowEffectMessage(effectValue, currentHealth);
            }
            else
            {
                UI.UIManager.Instance.ShowMessage1("Health is Full!");
            }

            // 调用内部类处理特效
            ParticleEffectManager.Instance.PlayParticleEffect("Heal",player,quaternion.identity, 
                Color.cyan,Color.green, 2f);
        }
    }
}   
