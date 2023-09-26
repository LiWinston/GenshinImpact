using UnityEngine;

namespace ItemSystem.Sub_strategy_and_gain_template
{
    public interface IItemEffectStrategy
    {
        void ApplyEffect(GameObject player);
    }
}