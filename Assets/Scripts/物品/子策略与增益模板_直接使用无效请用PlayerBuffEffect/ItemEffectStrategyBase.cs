using System;
using UnityEngine;

namespace AttributeRelatedScript
{
    public abstract class ItemEffectStrategyBase : IItemEffectStrategy
    {
        protected string effectType = null;
        protected string effectMessage;
        protected float effectValue;
        public ItemEffectStrategyBase(PlayerBuffEffect.EffectType et,float healAmount)
        {
            effectType = et.ToString();
            effectValue = healAmount;
            effectMessage = "Your " + effectType + " increased by {0}, now {1}!";
        }

        public abstract void ApplyEffect(GameObject player);

        protected void ShowEffectMessage(float effectValue, float currentValue)
        {
            string message = string.Format(effectMessage, effectValue, currentValue);
            Console.WriteLine(effectMessage);
            UI.UIManager.Instance.ShowMessage1(message);
        }
    }
}