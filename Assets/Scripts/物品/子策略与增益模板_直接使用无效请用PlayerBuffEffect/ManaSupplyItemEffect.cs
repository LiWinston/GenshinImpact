using System;
using Unity.Mathematics;
using UnityEngine;

namespace AttributeRelatedScript
{
    public class ManaSupplyItemEffect : ItemEffectStrategyBase
    {
        public ManaSupplyItemEffect(PlayerBuffEffect.EffectType et, float healAmount) : base(et, healAmount)
        {
        }

        public override void ApplyEffect(GameObject player)
        {
            player.GetComponent<Health>().RestoreEnergy(effectValue);
            float currentmana = player.GetComponent<Health>().CurrentEnergy; // 获取玩家当前的生命值
            ShowEffectMessage(effectValue, currentmana);
            ParticleEffectManager.Instance.PlayParticleEffect("MagicSupply",player,quaternion.identity, 
                Color.cyan,Color.green, 2f);
        }
    }
}