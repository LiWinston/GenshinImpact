using System;
using UnityEngine;

namespace AttributeRelatedScript
{
    public class ManaSupplyItemEffect : IItemEffectStrategy
    {
        [SerializeField]private float _increaseEng;
        public ManaSupplyItemEffect(float f)
        {
            _increaseEng = f;
        }

        public void ApplyEffect(GameObject player)
        {
            // 在这里实现恢复魔法值的逻辑
            player.GetComponent<Health>().RestoreEnergy(_increaseEng);
        }
    }
}