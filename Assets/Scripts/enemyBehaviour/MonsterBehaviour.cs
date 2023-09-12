using System;
using System.Collections;
using System.Collections.Generic;
using CodeMonkey.HealthSystemCM;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class MonsterBehaviour : MonoBehaviour
{
    public PlayerController targetPlayer;
    private Rigidbody rb;
    public Animator animator;
    private HealthSystem health;
    [SerializeField] private float mstForwardForce = 200;
    private float attackCooldownTimer;
    [SerializeField] private float attackCooldownInterval = 2f;
    private float moveForceTimerCounter;
    [SerializeField] private float moveForceCooldownInterval = 0.05f;
    [SerializeField] private float minAttackPower = 5;
    [SerializeField] private float maxAttackPower = 10;
    
     public float rotationSpeed = 0.000000001f; // 调整旋转速度
     
    // private float gameTime = Time.time;
    private float monsterLevel;
    private int monsterExperience;
    [SerializeField] private float aimDistance = 15;
    [SerializeField] private float chaseDistance = 8f;
    [SerializeField] private float stalkMstSpeed = 1f;
    [SerializeField] private float MaxMstSpeed = 2f;
    [SerializeField] private float stalkAccRatio = 0.8f;
    [SerializeField] private float attackDistance = 1.5f;
    private bool isMoving;
    private State _state;
    private float curDistance;

    [InspectorLabel("Freeze")]
    private bool isFrozen = false; // 表示怪物是否处于冰冻状态
    private float originalMoveForce;
    private float originalAttackCooldownInterval;
    private float originalMaxMstSpeed;
    
    private void Start()
    {
        targetPlayer = GameObject.Find("Player").GetComponent<PlayerController>();

        if (targetPlayer == null)
        {
            Debug.LogWarning("No GameObject with the name 'Player' found in the scene.");
        }
        _state = targetPlayer.GetComponent<State>();
        animator = GetComponentInChildren<Animator>();
        if (targetPlayer == null)
        {
            Debug.LogWarning("No Animator Compoment found.");
        }
        // 在子对象中查找 Rigidbody
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
        originalMoveForce = mstForwardForce;
        originalAttackCooldownInterval = attackCooldownInterval;
        originalMaxMstSpeed = MaxMstSpeed;
        // 初始化怪物经验值和等级
        InitializeMonsterLevel();
        
    }


    private void Update()
    {
        if (health.IsDead())
        {
            _state.AddExperience(this.monsterExperience);
            targetPlayer.showExp("EXP " + this.monsterExperience);
            StartCoroutine(nameof(PlayDeathEffects));
            return;
        }
        
        isMoving = rb.velocity.magnitude > 0.1f;
        animator.SetBool("isMoving", isMoving);

        // Decrease the move force cooldown timer
        moveForceTimerCounter -= Time.deltaTime;

        // Decrease the attack cooldown timer
        attackCooldownTimer -= Time.deltaTime;

        curDistance = Vector3.Distance(transform.position, targetPlayer.transform.position);
        if (curDistance <= aimDistance && curDistance > attackDistance)
        {
            if (curDistance > chaseDistance)
            {
                if (rb.velocity.magnitude < stalkMstSpeed)
                {
                    rb.AddForce(transform.forward * (stalkAccRatio * mstForwardForce), ForceMode.Force);
                    moveForceTimerCounter = moveForceCooldownInterval;
                }
            }
            else if (curDistance <= chaseDistance)
            {
                if (rb.velocity.magnitude < MaxMstSpeed)
                {
                    rb.AddForce(transform.forward * mstForwardForce, ForceMode.Force);
                    moveForceTimerCounter = moveForceCooldownInterval;
                }
            }
        }
        else if (targetPlayer && curDistance < attackDistance && attackCooldownTimer <= 0)
        {
            Attack();
            attackCooldownTimer = attackCooldownInterval;
        }
        else
        {
            animator.SetBool("Near",false);
        }
    }

    void FixedUpdate()
    {
        if (curDistance <= aimDistance)
        {
            animator.SetBool("Near",false);
            
            var directionToPly = targetPlayer.transform.position - transform.position;
            directionToPly.y = 0;
            directionToPly.Normalize();
            Quaternion targetRotation = Quaternion.LookRotation(directionToPly);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }
    
     private void Attack()
    {
        // UIManager.Instance.ShowMessage1("揍你！");
        animator.SetTrigger("AttackTrigger");
        targetPlayer.TakeDamage(monsterLevel/20 * Random.Range(minAttackPower, maxAttackPower));
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
        // 计算怪物等级，使其在五分钟内逐渐增长到最大等级
        float maxGameTime = 400f; // 300秒
        float progress = Mathf.Clamp01(Time.time / maxGameTime); // 游戏时间进度（0到1之间）
        monsterLevel = progress * 100 + 1; // 从1到100逐渐增长
        monsterExperience = Mathf.FloorToInt(monsterLevel * 1.2f);
        health.SetHealthMax(monsterLevel * 100 +100, true);
    }
    
    public IEnumerator ApplyFreezeEffect(float duration)
    {
        if (!isFrozen)
        {
            isFrozen = true;
            rb.velocity *= 0.1f;
            // 减小加速推力和增加攻击间隔
            mstForwardForce *= 0.6f; // 降低至60%
            attackCooldownInterval *= 2f; // 增加至200%
            MaxMstSpeed *= 0.36f;
            // 等待冰冻效果持续时间
            yield return new WaitForSeconds(duration);

            // 恢复原始推力和攻击间隔
            mstForwardForce = originalMoveForce;
            attackCooldownInterval = originalAttackCooldownInterval;
            MaxMstSpeed = originalMaxMstSpeed;
            isFrozen = false;
        }
    }
}
