using System.Collections;
using System.Collections.Generic;
using AttributeRelatedScript;
using CodeMonkey.HealthSystemCM;
using UnityEngine;
using UnityEngine.Serialization;

public class SpellCast : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private Transform spellingPartTransform; // 序列化字段，用于拖放 Weapon 物体
    [SerializeField] private float spellRange = 1.6f;
    private State state;


    void Start()
    {
        state = GetComponent<State>();
        if (spellingPartTransform == null)
        {
            Debug.LogError("Weapon Transform 未指定，请在 Inspector 中将 Weapon 物体拖放到该字段中！");
        }

        var childTransform = transform.Find("Model");
        if (childTransform != null)
        {
            animator = childTransform.GetComponent<Animator>();
        }

        if (animator == null)
        {
            Debug.LogError("找不到 Animator 组件！");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            animator.SetTrigger("Cast");
            CastSpell();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            animator.SetTrigger("ULT");
            CastUlt();
        }
    }

    
    private void CastSpell()
    {
        //TODO:更新此机制。
        if (!state.ConsumeEnergy(2))
        {
            return;
        };
        // 检查是否成功获取了 Weapon 物体的引用
        if (spellingPartTransform != null)
        {
            ParticleEffectManager.Instance.PlayParticleEffect("Spell", spellingPartTransform.gameObject,
                Quaternion.identity,
                Color.white, Color.white, 1f);
        }
        else
        {
            Debug.LogError("无法播放特效，因为 Weapon Transform 未指定！");
        }
        // 获取玩家的位置
        Vector3 playerPosition = transform.position;

        // 检测在法术范围内的敌人
        Collider[] hitEnemies = Physics.OverlapSphere(playerPosition, spellRange);

        foreach (Collider enemy in hitEnemies)
        {
            // 检查是否敌人
            if (enemy.CompareTag("Enemy"))
            {
                // 获取敌人的 HealthSystem 组件
                HealthSystem enemyHealth = enemy.GetComponent<HealthSystemComponent>().GetHealthSystem();

                if (enemyHealth != null)
                {
                    // 对敌人造成伤害
                    enemyHealth.Damage(state.CurrentDamage);

                    // 播放特效
                    if (spellingPartTransform != null)
                    {
                        Transform spineTransform = FindDeepChild(enemy.transform, "spine_01");
                        if (spineTransform != null)
                        {
                            // 找到了 spine_01 子物体，可以使用它的Transform
                            ParticleEffectManager.Instance.PlayParticleEffect("HitBySpell", spineTransform.gameObject, Quaternion.identity,
                                Color.red, Color.black, 1f);
                        }
                        else
                        {
                            // 未找到 spine_01，可以执行默认逻辑
                            ParticleEffectManager.Instance.PlayParticleEffect("HitBySpell", enemy.gameObject, Quaternion.identity,
                                Color.red, Color.black, 1f);
                        }
                        // ParticleEffectManager.Instance.PlayParticleEffect("HitBySpell", enemy.gameObject,
                        //     Quaternion.identity,
                        //     Color.cyan, Color.green, 1f);
                    }
                }
            }
        }
    }
    
    private void CastUlt()
    {
        //TODO:更新此机制。
        if (!state.ConsumeEnergy(10))
        {
            return;
        };
        // 检查是否成功获取了 Weapon 物体的引用
        if (spellingPartTransform != null)
        {
            ParticleEffectManager.Instance.PlayParticleEffect("ULT", spellingPartTransform.gameObject,
                Quaternion.identity,
                Color.white, Color.white, 1.2f);
        }
        else
        {
            Debug.LogError("无法播放特效，因为 Weapon Transform 未指定！");
        }
        // 获取玩家的位置
        Vector3 playerPosition = transform.position;

        // 检测在法术范围内的敌人
        Collider[] hitEnemies = Physics.OverlapSphere(playerPosition, spellRange);

        foreach (Collider enemy in hitEnemies)
        {
            // 检查是否敌人
            if (enemy.CompareTag("Enemy"))
            {
                // 获取敌人的 HealthSystem 组件
                HealthSystem enemyHealth = enemy.GetComponent<HealthSystemComponent>().GetHealthSystem();

                if (enemyHealth != null)
                {
                    // 对敌人造成伤害
                    enemyHealth.Damage(state.CurrentDamage * 4);

                    // 播放特效
                    if (spellingPartTransform != null)
                    {
                        Transform spineTransform = FindDeepChild(enemy.transform, "spine_01");
                        if (spineTransform != null)
                        {
                            // 找到了 spine_01 子物体，可以使用它的Transform
                            ParticleEffectManager.Instance.PlayParticleEffect("HitByUlt", spineTransform.gameObject, Quaternion.identity,
                                Color.white, Color.blue, 1.8f);
                        }
                        else
                        {
                            // 未找到 spine_01，可以执行默认逻辑
                            ParticleEffectManager.Instance.PlayParticleEffect("HitByUlt", enemy.gameObject, Quaternion.identity,
                                Color.white, Color.blue, 1.8f);
                        }
                        // ParticleEffectManager.Instance.PlayParticleEffect("HitBySpell", enemy.gameObject,
                        //     Quaternion.identity,
                        //     Color.cyan, Color.green, 1f);
                    }
                }
            }
        }
    }
    
    
    /// <summary>
    /// Utility function used to seek into a subclass for a Transform
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Transform FindDeepChild(Transform parent, string name)
    {
        Transform result = parent.Find(name);
        if (result != null)
        {
            return result;
        }

        foreach (Transform child in parent)
        {
            result = FindDeepChild(child, name);
            if (result != null)
            {
                return result;
            }
        }

        return null; // 没有找到
    }
}