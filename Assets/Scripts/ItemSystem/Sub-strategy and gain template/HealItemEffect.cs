using System.Collections.Generic;
using AttributeRelatedScript;
using Unity.Mathematics;
using UnityEngine;
using Utility;

namespace ItemSystem.Sub_strategy_and_gain_template
{
    public class HealItemEffect : ItemEffectStrategyBase
    {
        public HealItemEffect(PlayerBuffEffect.EffectType et, float healAmount) : base(et, healAmount)
        {
        }

        public override void ApplyEffect(GameObject player)
        {
            // 在这里实现恢复生命值的逻辑
            State.Instance.Heal(effectValue); // 使用字段来指定恢复的生命值数量
            State.Instance.IsInCombat = false;
            float currentHealth = player.GetComponent<State>().CurrentHealth; // 获取玩家当前的生命值
            if (!player.GetComponent<State>().IsFullHealth())
            {
                ShowEffectMessage(effectValue, currentHealth);
            }
            else
            {
                UI.UIManager.Instance.ShowMessage1("Health is Full!");
            }

            SoundEffectManager.Instance.PlaySound("Music/音效/法术/武_云蒸霞蔚", player);
            // 调用内部类处理特效
            ParticleEffectManager.Instance.PlayParticleEffect("Heal",player,quaternion.identity, 
                Color.cyan,Color.green, 2f);
        }
    }
}   
