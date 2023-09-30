using System.Collections;
using Behavior.Effect;
using Behavior.Health;

using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using Utility;
using IPoolable = Utility.IPoolable;
using Random = UnityEngine.Random;
using State = AttributeRelatedScript.State;
using Target = UI.OffScreenIndicator.Target;

namespace Behavior
{
    public class MonsterBehaviour : MonoBehaviour, IPoolable
    {
        public PlayerController targetPlayer;
        private GameObject target;
        private LayerMask enemyLayer;
        private LayerMask playerLayer;
    

        private Rigidbody rb;
        public Animator animator;
        internal HealthSystem health;
        [SerializeField] private float mstForwardForce = 200;
        private float attackCooldownTimer;
        [SerializeField] private float attackCooldownInterval = 2f;
        private float moveForceTimerCounter;
        [SerializeField] private float moveForceCooldownInterval = 0.05f;
        private float obstacleDetectionTimer = 0f;
        public float obstacleDetectionInterval = 3f; // 检测间隔，每隔3秒检测一次
    
        [SerializeField] private float minAttackPower = 5;
        [SerializeField] private float maxAttackPower = 10;
    
    
        public float rotationSpeed = 2f; // 调整旋转速度
     
        // private float gameTime = Time.time;
        internal float monsterLevel;
        private int monsterExperience;
        [SerializeField] private float aimDistance;
        [SerializeField] private float chaseDistance;
        // [SerializeField] private float stalkMstSpeed = 1f;
        [SerializeField] private float MaxMstSpeed = 2f;
        // [SerializeField] private float stalkAccRatio = 0.8f;
        [SerializeField] private float attackDistance = 1.5f;
        private bool isMoving;
        private State _state;
        private float curDistance;

        [InspectorLabel("Freeze")]
        private bool isFrozen; // 表示怪物是否处于冰冻状态
        private float originalMoveForce;
        private float originalAttackCooldownInterval;
        private float originalMaxMstSpeed;
    
        
        private Target _targetComponent;
        private static readonly int Die = Animator.StringToHash("Die");
        internal EffectTimeManager _effectTimeManager;

        public ObjectPool<GameObject> ThisPool { get; set; }
        public bool IsExisting { get; set; }

        public void SetPool(UnityEngine.Pool.ObjectPool<GameObject> pool)
        {
            ThisPool = pool;
        }

        public void actionOnGet()
        {
            gameObject.SetActive(true);
            InitializeMonsterLevel();
            target = PlayerController.Instance.gameObject;
            health.SetHealthMax(monsterLevel * 100 +100, true);
        }

        public void actionOnRelease()
        {
            rb.velocity = Vector3.zero;
            if(!freezeEffectCoroutine.IsUnityNull())StopCoroutine(freezeEffectCoroutine);
            IsInSelfKill = false;
            _targetComponent.targetColor = Color.red;
        }

        public void Release()
        {
            ThisPool.Release(gameObject);
        }


        private void Awake()
        {
            _effectTimeManager = GetComponent<EffectTimeManager>();
            target = PlayerController.Instance.gameObject;
            enemyLayer = LayerMask.GetMask("Enemy");
            playerLayer = LayerMask.GetMask("Player");
            _targetComponent = GetComponent<Target>();
        }
    
        private void Start()
        {
            target = PlayerController.Instance.gameObject;
            targetPlayer = PlayerController.Instance;

            if(_effectTimeManager == null) _effectTimeManager = GetComponent<EffectTimeManager>();
            if (targetPlayer == null)
            {
                Debug.LogWarning("No GameObject with the name 'Player' found in the scene.");
            }
            _state = targetPlayer.GetComponent<State>();
            animator = GetComponentInChildren<Animator>();
            if (animator == null)
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
                targetPlayer.ShowPlayerHUD("EXP " + this.monsterExperience);
                DeactiveAllEffect();
                StartCoroutine(nameof(PlayDeathEffects));
                return;
            }
        
            isMoving = rb.velocity.magnitude > 0.01f;
            animator.SetBool("isMoving", isMoving);

            // Decrease the move force cooldown timer
            moveForceTimerCounter -= Time.deltaTime;

            // Decrease the attack cooldown timer
            attackCooldownTimer -= Time.deltaTime;

            if (IsInSelfKill)
            {
                target = PickAlly();//距离更新已经写在择敌方法中
            }
            else
            {
                target = targetPlayer.GameObject();
                curDistance = Vector3.Distance(transform.position, target.transform.position);
            }
        
            if (curDistance <= chaseDistance && curDistance > attackDistance)
            {
                obstacleDetectionTimer -= Time.deltaTime;

                isMoving = rb.velocity.magnitude > 0.01f;
                animator.SetBool("isMoving", isMoving);

                // 如果计时器小于等于0，进行障碍物检测
                if (obstacleDetectionTimer <= 0f)
                {
                    // 在此处进行障碍物检测逻辑，包括尝试跳跃和避免障碍物的移动逻辑
                    ObstacleHandle();

                    // 重置计时器
                    obstacleDetectionTimer = obstacleDetectionInterval;
                }
                if (rb.velocity.magnitude < MaxMstSpeed)
                {
                    rb.AddForce(transform.forward * mstForwardForce, ForceMode.Force);
                    moveForceTimerCounter = moveForceCooldownInterval;
                }
            }
            else if (target && curDistance < attackDistance && attackCooldownTimer <= 0)
            {
                Attack();
                attackCooldownTimer = attackCooldownInterval;
            }
            else//追击距离外 瞄准距离内
            {
                animator.SetBool("Near",false);
            }
        }

        private GameObject PickAlly()
        {
            if (target.layer == enemyLayer && !target.GetComponent<MonsterBehaviour>().health.IsDead())
            {
                Debug.LogWarning("保持原敌");
                return target;
            }
            // 获取所有在怪物周围的敌人
            Collider[] nearEnemies = Physics.OverlapSphere(transform.position, chaseDistance, enemyLayer);

            // 初始化最近敌人和最近距离
            GameObject nearestEnemy = null;
            float nearestDistance = float.MaxValue;

            // 遍历所有附近的敌人
            foreach (Collider enemyCollider in nearEnemies)
            {
                if (enemyCollider.gameObject == gameObject) continue;
                // 检查敌人是否存活
                MonsterBehaviour enemyMonster = enemyCollider.GetComponent<MonsterBehaviour>();
                if (enemyMonster != null && !enemyMonster.health.IsDead())
                {
                    // 计算距离
                    float distance = Vector3.Distance(transform.position, enemyCollider.transform.position);

                    // 如果找到更近的敌人，更新最近敌人和距离
                    if (distance < nearestDistance)
                    {
                        nearestEnemy = enemyCollider.gameObject;
                        nearestDistance = distance;
                    }
                }
            }
            curDistance = nearestDistance;
        
            if (nearestEnemy != null)
            {
                return nearestEnemy;
            }
            else
            {
                // 如果没有找到最近的敌人，返回当前目标
                return gameObject;
            }
        }

        private void ObstacleHandle()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 0.4f))
            {
                // 如果检测到障碍物，施加向上的力以跳跃
                rb.AddForce(Vector3.up * 1000, ForceMode.Impulse); // 添加跳跃力
                rb.AddForce(transform.forward * mstForwardForce, ForceMode.Impulse); // 添加向前的力
            }
        }


        void FixedUpdate()
        {
            if (curDistance <= aimDistance) //追击距离内
            {
                animator.SetBool("Near",true);
            
                var directionToPly = target.transform.position - transform.position;
                directionToPly.y = 0;
                directionToPly.Normalize();
                Quaternion targetRotation = Quaternion.LookRotation(directionToPly);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            }
        }
    
        private void Attack()
        {
            // UIManager.Instance.ShowMessage1("揍你！");
            if (!IsInSelfKill)
            {
                animator.SetTrigger("AttackTrigger");
                targetPlayer.TakeDamage(5 + monsterLevel/20 * Random.Range(minAttackPower, maxAttackPower));
            }
            else
            {
                if (target.gameObject != gameObject)
                {
                    animator.SetTrigger("AttackTrigger");
                }
                target.GetComponent<MonsterBehaviour>().TakeDamage(70 + monsterLevel/20 * Random.Range(minAttackPower, maxAttackPower));
            }
        }

        internal void TakeDamage(float dmg)
        {
            animator.SetTrigger("Hurt");
            health.Damage(dmg);
        }

        private IEnumerator PlayDeathEffects()
        {
            animator.SetTrigger(Die);
            ParticleEffectManager.Instance.PlayParticleEffect("MonsterDie", this.gameObject, Quaternion.identity, Color.red, Color.black, 1.2f);
            yield return new WaitForSeconds(1.2f);
            // Destroy(this.gameObject);
            Release();
        }

        //TODO:逻辑待更新。
        private void InitializeMonsterLevel()
        {
            // 计算怪物等级，使其在五分钟内逐渐增长到最大等级
            float maxGameTime = 300f; // 300秒
            float progress = Mathf.Clamp01(Time.time / maxGameTime); // 游戏时间进度（0到1之间）
            monsterLevel = progress * 100 + 1; // 从1到100逐渐增长
            monsterExperience = Mathf.FloorToInt(monsterLevel * 1.2f);
            health.SetHealthMax(monsterLevel * 300 +100, true);//100
        }
    
        public void ActivateFreezeMode(float duration, float continuousDamageAmount)
        {
            freezeEffectCoroutine = StartCoroutine(FreezeEffectCoroutine(duration));
                    // 启动持续掉血的协程
            StartCoroutine(Effect.Freeze.ContinuousDamage(health, continuousDamageAmount, duration ));
            _effectTimeManager.CreateEffectBar("Freeze", Color.blue, duration);
        }
        internal Coroutine freezeEffectCoroutine { get; set; }

        public void DeactivateFreezeMode()
        {
            if(!freezeEffectCoroutine.IsUnityNull()) StopCoroutine(freezeEffectCoroutine);
            // 恢复原始推力和攻击间隔
            mstForwardForce = originalMoveForce;
            attackCooldownInterval = originalAttackCooldownInterval;
            MaxMstSpeed = originalMaxMstSpeed;
            isFrozen = false;
            _effectTimeManager.StopEffect("Freeze");
        }
        private IEnumerator FreezeEffectCoroutine(float duration)
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


    
        public void ActivateSelfKillMode(float elapseT)
        {
            if(IsInSelfKill) StopCoroutine(selfKillCoroutine);
            selfKillCoroutine = StartCoroutine(SelfKillCoroutine(elapseT));
            // Debug.Log("SelfKillMode Activated");
            _effectTimeManager.CreateEffectBar("SelfKill", Color.white, elapseT);
            // Debug.Log("SelfKill timerCpn Activated");
        }

        public void DeactivateSelfKillMode()
        {
            IsInSelfKill = false;
            _effectTimeManager.StopEffect("SelfKill");
            GetComponent<Target>().NeedBoxIndicator = false;
            MaxMstSpeed = originalMaxMstSpeed;
            if(!selfKillCoroutine.IsUnityNull()) StopCoroutine(selfKillCoroutine);
        }
        public Coroutine selfKillCoroutine { get; set; }

        private IEnumerator SelfKillCoroutine(float elapseT)
        {
            IsInSelfKill = true;
            MaxMstSpeed *= 1.2f;
            
            // var orgTargetColor = targetComponent.targetColor;
            // Color startColor = Color.green;
            float elapsedTime = 0f;

            while (elapsedTime < elapseT)
            {
                // float t = Mathf.Clamp01(elapsedTime / elapseT);
                // targetComponent.targetColor = Color.Lerp(startColor, orgTargetColor, t);

                yield return new WaitForSeconds(1f); // 等待1秒钟
                elapsedTime += 1f;
            }

            // Ensure the final color is exactly the original color.
            // targetComponent.targetColor = orgTargetColor;

            yield return null; // 保证协程执行完整

            IsInSelfKill = false;
        }

        public bool IsInSelfKill { get; set; }
        private void DeactiveAllEffect()
        {
            DeactivateSelfKillMode();
            DeactivateFreezeMode();
        }
    }
}
