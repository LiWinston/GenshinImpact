using System.Collections;
using System.Collections.Generic;
using CodeMonkey.HealthSystemCM;
using UI;
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
    
    private float gameTime = 0;
    private int monsterLevel;
    private int monsterExperience;

    private void Start()
    {
        
        targetPlayer = FindObjectOfType<PlayerController>();
        if (targetPlayer == null)
        {
            Debug.LogWarning("PlayerController not found on parent or in the scene.");
        }

        // 在父空对象及其所有子对象中查找 Rigidbody
        rb = GetComponentInChildren<Rigidbody>();
        if (rb != null)
        {
            // UIManager.ShowMessage2("MST 内部 Rigidbody找到.");
        }

        // 获取 HealthSystemComponent，并从中获取 HealthSystem
        HealthSystemComponent healthSystemComponent = GetComponent<HealthSystemComponent>();
        if (healthSystemComponent != null)
        {
            health = healthSystemComponent.GetHealthSystem();
            // UIManager.ShowMessage2("health 已找到.");
        }
        // 初始化怪物经验值和等级
        InitializeMonsterLevel();
    }



     void Update()
    {
        gameTime += Time.deltaTime;
        
        if (health.IsDead())
        {
            targetPlayer.GetComponent<State>().AddExperience(this.monsterExperience);
            UIManager.ShowExp("EXP " + this.monsterExperience);
            StartCoroutine(nameof(PlayDeathEffects));
            return;
        }

        // Decrease the move force cooldown timer
        moveForceTimerCounter -= Time.deltaTime;

        // Decrease the attack cooldown timer
        attackCooldownTimer -= Time.deltaTime;

        float distanceTemp = Vector3.Distance(transform.position, targetPlayer.transform.position);

        if (distanceTemp <= chaseDistance)
        {
            var directionToPly = targetPlayer.transform.position - transform.position;
            directionToPly.y = 0;
            directionToPly.Normalize();
            transform.forward = directionToPly;

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

    //TODO:逻辑待更新。
    private void InitializeMonsterLevel()
    {
        // 计算怪物等级，使其在五分钟内逐渐增长到最大等级（例如，最大等级为10）
        float maxGameTime = 300f; // 五分钟共300秒
        float progress = Mathf.Clamp01(gameTime / maxGameTime); // 游戏时间进度（0到1之间）
        monsterLevel = Mathf.FloorToInt(progress * 10) + 1; // 从1到10逐渐增长
        monsterExperience = (int)(monsterLevel * 1.4f);
    }
}