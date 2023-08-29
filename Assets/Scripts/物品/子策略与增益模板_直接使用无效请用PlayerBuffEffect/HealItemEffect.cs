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
        }
    }
}