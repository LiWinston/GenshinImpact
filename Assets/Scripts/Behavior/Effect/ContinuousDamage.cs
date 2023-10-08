using System.Collections;
using Behavior.Health;
using UnityEngine;

namespace Behavior.Effect
{
    public static class ContinuousDamage
    {
        public static IEnumerator MakeContinuousDamage(GameObject obj, float damageAmount, float continuousDamageDuration = 3.0f)
        {
            switch (obj.tag)
            {
                case "Player":
                    return MakeContinuousDamage(obj.GetComponent<PlayerController>(), damageAmount, continuousDamageDuration);
                case "Enemy":
                    return MakeContinuousDamage(obj.GetComponent<MonsterBehaviour>().health , damageAmount, continuousDamageDuration);
            }
            return null;
        }
        public static IEnumerator MakeContinuousDamage(HealthSystem enemyHealth, float damageAmount, float continuousDamageDuration = 3.0f)
        {
            // 持续掉血的时间，可以根据需要进行调整
            float timer = 0f;
        
            while (timer < continuousDamageDuration)
            {
                // 对敌人造成持续伤害
                enemyHealth.Damage(damageAmount * Time.deltaTime);
            
                // 等待一帧
                yield return null;
                timer += Time.deltaTime;
                if (enemyHealth.IsDead()) break;
            }
        }
        public static IEnumerator MakeContinuousDamage(PlayerController ply, float damageAmount, float continuousDamageDuration = 3.0f)
        {
            // 持续掉血的时间，可以根据需要进行调整
            float timer = 0f;
        
            while (timer < continuousDamageDuration)
            {
                // 对敌人造成持续伤害
                ply.TakeDamage(damageAmount * Time.deltaTime, true);
            
                // 等待一帧
                yield return null;
                if (ply.state.IsEmptyHealth()) break;
                timer += Time.deltaTime;
            }
        }
        
    }
}