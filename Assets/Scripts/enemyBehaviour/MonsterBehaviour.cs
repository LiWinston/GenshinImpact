using System.Collections;
using System.Collections.Generic;
using CodeMonkey.HealthSystemCM;
using UI;
using UnityEngine;
using UnityEngine.Serialization;

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
     
    private float gameTime = 0;
    private int monsterLevel;
    private int monsterExperience;
    [SerializeField] private float aimDistance = 15;
    [SerializeField] private float chaseDistance = 8f;
    [SerializeField] private float stalkMstSpeed = 1f;
    [SerializeField] private float MaxMstSpeed = 2f;
    [SerializeField] private float stalkAccRatio = 0.8f;
    [SerializeField] private float attackDistance = 1.5f;
    private bool isMoving;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        if (targetPlayer == null)
        {
            Debug.LogWarning("No Animator Compoment found.");
        }
        targetPlayer = GameObject.Find("Player").GetComponent<PlayerController>();

        if (targetPlayer == null)
        {
            Debug.LogWarning("No GameObject with the name 'Player' found in the scene.");
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
        InitializeMonsterLevel();
    }



     void FixedUpdate()
    {
        gameTime += Time.deltaTime;
        
        if (health.IsDead())
        {
            targetPlayer.GetComponent<State>().AddExperience(this.monsterExperience);
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

        float curDistance = Vector3.Distance(transform.position, targetPlayer.transform.position);

        if (curDistance <= aimDistance && curDistance > attackDistance)
        {
            animator.SetBool("Near",false);
            
            var directionToPly = targetPlayer.transform.position - transform.position;
            directionToPly.y = 0;
            directionToPly.Normalize();
            Quaternion targetRotation = Quaternion.LookRotation(directionToPly);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            
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


     private void Attack()
    {
        // UIManager.Instance.ShowMessage1("揍你！");
        animator.SetTrigger("AttackTrigger");
        targetPlayer.TakeDamage(Random.Range(minAttackPower, maxAttackPower));
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
