using AttributeRelatedScript;
using Unity.Mathematics;
using UnityEngine;
using Utility;

namespace ItemSystem.Sub_strategy_and_gain_template
{
    public class DamageIncreaseItemEffect : ItemEffectStrategyBase
    {
        public DamageIncreaseItemEffect(PlayerBuffEffect.EffectType et, float healAmount) : base(et, healAmount)
        {
            
        }

        public override void ApplyEffect(GameObject player)
        {
            SoundEffectManager.Instance.PlaySound("Music/音效/战斗/攻击提升", player);
            player.GetComponent<State>().IncreaseDamage(effectValue);
            ShowEffectMessage(effectValue, player.GetComponent<State>().CurrentDamage);
            var pos = Find.FindDeepChild(player.transform, "spine_03");
            ParticleEffectManager.Instance.PlayParticleEffect("DamageUp",pos.gameObject,quaternion.identity, 
                Color.cyan,Color.green, 2.8f);
        }
    }
}