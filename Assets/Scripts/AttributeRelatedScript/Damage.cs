using UnityEngine;

namespace AttributeRelatedScript
{
    public class Damage : MonoBehaviour
    {
        public float damage = 10f;
        public void IncreaseDamage(float idmg)
        {
            damage += idmg;
            
        }
        public float CurrentDamage
        {
            get=>damage;
            set =>damage = value;// 在设置 CurrentHealth 时，确保值不超出最大生命值范围
        }
    }
}