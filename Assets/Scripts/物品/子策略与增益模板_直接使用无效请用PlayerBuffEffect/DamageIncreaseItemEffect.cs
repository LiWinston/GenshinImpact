using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace AttributeRelatedScript
{
    public class DamageIncreaseItemEffect : ItemEffectStrategyBase
    {
        public DamageIncreaseItemEffect(PlayerBuffEffect.EffectType et, float healAmount) : base(et, healAmount)
        {
            
        }

        public override void ApplyEffect(GameObject player)
        {
            // 在这里实现增加伤害的逻辑
            player.GetComponent<Damage>().IncreaseDamage(effectValue);
            ShowEffectMessage(effectValue, player.GetComponent<Damage>().CurrentDamage);
        }
    }
}