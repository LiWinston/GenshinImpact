using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace AttributeRelatedScript
{
    public class DamageIncreaseItemEffect : IItemEffectStrategy
    {
        [SerializeField]private float _increaseDmg;

        public DamageIncreaseItemEffect(float increaseDmg)
        {
            _increaseDmg = increaseDmg;
        }

        public void ApplyEffect(GameObject player)
        {
            // 在这里实现增加伤害的逻辑
            player.GetComponent<Damage>().IncreaseDamage(_increaseDmg);
            UI.UIManager.ShowMessage1("Your damage increased by " + _increaseDmg +" !");
        }
    }
}