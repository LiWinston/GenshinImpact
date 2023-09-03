using System.Collections;
using System.Collections.Generic;
using CodeMonkey.HealthSystemCM;
using UnityEngine;

public class MonsterBehaviour : MonoBehaviour
{
    public PlayerController targetPlayer;
    private Rigidbody rb; // 引用怪物的刚体组件
    [SerializeField]private float attackDistance = 0.7f;
    private HealthSystem health;
    [SerializeField] private float chaseDistance = 10;
    [SerializeField] private float mstForwardForce = 40;
    private float attackCooldownTimer;
    [SerializeField] private float attackCooldownInterval = 2f;
    private float moveForceTimerCounter;
    [SerializeField] private float moveForceCooldownInterval = 0.05f;
    [SerializeField] private float minAttackPower = 1;
    [SerializeField] private float maxAttackPower = 3;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // 获取怪物的刚体组件
        health = GetComponent<HealthSystemComponent>().GetHealthSystem();
    }

    void Update()
    {
        if (health.IsDead())
        {
            StartCoroutine(nameof(PlayDeathEffects));
            return;
        }

        // Decrease the move force cooldown timer
        moveForceTimerCounter -= Time.deltaTime;
    
        // Decrease the attack cooldown timer
        attackCooldownTimer -= Time.deltaTime;

        if (!targetPlayer)
        {
            targetPlayer = FindObjectOfType<PlayerController>();//查找场景中第一个具有指定类型的组件的对象
        }
        else
        {
            float distanceTemp = Vector3.Distance(transform.position, targetPlayer.transform.position);

            if (distanceTemp <= chaseDistance)
            {
                transform.forward = targetPlayer.transform.position - transform.position;

                if (distanceTemp > attackDistance)
                {
                    // Check if the move force cooldown has expired
                    if (moveForceTimerCounter <= 0)
                    {
                        rb.AddForce(transform.forward * mstForwardForce, ForceMode.Impulse);
                        // Reset the move force cooldown timer
                        moveForceTimerCounter = moveForceCooldownInterval;
                    }
                }

                // Check if the attack cooldown has expired
                if (targetPlayer && distanceTemp < attackDistance && attackCooldownTimer <= 0)
                {
                    Attack();
                    // Reset the attack cooldown timer
                    attackCooldownTimer = attackCooldownInterval;
                }
            }
        }
    }
    

    private void Attack()
    {
        targetPlayer.GetComponent<State>().TakeDamage(Random.Range(minAttackPower, maxAttackPower));
    }

    private IEnumerator PlayDeathEffects()
    {
        ParticleEffectManager.Instance.PlayParticleEffect("MonsterDie", this.gameObject, Quaternion.identity, Color.red, Color.black, 1.2f);
        yield return new WaitForSeconds(1.2f);
        Destroy(this.gameObject);
    }



    
}
