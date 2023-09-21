using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Utility;

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
            player.GetComponent<State>().IncreaseDamage(effectValue);
            ShowEffectMessage(effectValue, player.GetComponent<State>().CurrentDamage);
            var pos = Find.FindDeepChild(player.transform, "spine_03");
            ParticleEffectManager.Instance.PlayParticleEffect("DamageUp",pos.gameObject,quaternion.identity, 
                Color.cyan,Color.green, 2.8f);
        }
    }
}