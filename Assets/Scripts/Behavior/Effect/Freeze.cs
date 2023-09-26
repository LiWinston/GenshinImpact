using System.Collections;
using Behavior.Health;
using UnityEngine;

namespace Behavior.Effect
{
    public class Freeze
    {
        public static IEnumerator ContinuousDamage(HealthSystem enemyHealth, float damageAmount, float continuousDamageDuration = 3.0f)
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
            }
        }
        public static IEnumerator ContinuousDamage(PlayerController ply, float damageAmount, float continuousDamageDuration = 3.0f)
        {
            // 持续掉血的时间，可以根据需要进行调整
            float timer = 0f;
        
            while (timer < continuousDamageDuration)
            {
                // 对敌人造成持续伤害
                ply.TakeDamage(damageAmount * Time.deltaTime);
            
                // 等待一帧
                yield return null;
            
                timer += Time.deltaTime;
            }
        }
        
    }
}