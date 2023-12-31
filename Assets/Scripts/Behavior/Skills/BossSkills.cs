using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using IPoolable = Utility.IPoolable;

namespace Behavior.Skills
{
    public class BossSkill : MonoBehaviour
    {
        public GameObject projectilePrefab; // 远程投射物的预制体
        public Transform projectileSpawnPoint; // 投射物生成点
        public float attackCooldown = 0.8f; // 攻击冷却时间（每次攻击之间的间隔）
        public float aimDistance = 10f; // 瞄准玩家的距离
        private float atkDistance;
        private Transform playerTransform;
        private float attackCooldownTimer;
        public MonsterBehaviour _monsterBehaviour;
        protected ObjectPool<GameObject> _throwingsPool;
        [SerializeField] private int defaultCapacity = 8;
        [SerializeField] private int maxCapacity = 12;

        private void Awake()
        {
            _monsterBehaviour = GetComponent<MonsterBehaviour>();
            attackCooldownTimer = attackCooldown;
        }

        private void Start()
        {
            _throwingsPool = new ObjectPool<GameObject>(CreateFunc, actionOnGet, actionOnRelease, actionOnDestroy,
                true, defaultCapacity, maxCapacity);
            // 获取玩家的Transform
            playerTransform = PlayerController.Instance.transform;
            atkDistance = _monsterBehaviour.attackDistance;
        }
        private GameObject CreateFunc(){
            GameObject throwing = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            actionOnGet(throwing);
            throwing.GetComponent<IPoolable>().SetPool(_throwingsPool);
            return throwing.GameObject();
        }

        private void actionOnGet(GameObject obj){
            obj.transform.position = projectileSpawnPoint.position;
            obj.transform.forward = _monsterBehaviour.transform.forward;
            obj.GetComponent<MonsterProjectile>()._monsterBehaviour = _monsterBehaviour;
            obj.GetComponent<IPoolable>().actionOnGet();
        }

        private void actionOnRelease(GameObject obj)
        {
            obj.GetComponent<IPoolable>().actionOnRelease();
            obj.SetActive(false);
        }

        void actionOnDestroy(GameObject obj)
        {
            // Destroy(obj);
        }

        private void Update()
        {
            // 检查是否可以进行攻击
            if (CanAttack())
            {
                // 瞄准玩家
                AimAtPlayer();

                // 发射投射物
                ShootProjectile();

                // 重置攻击冷却计时器
                // attackCooldownTimer = _monsterBehaviour.attackCooldownInterval;
            }
            else
            {
                // 减少攻击冷却计时器
                attackCooldownTimer -= Time.fixedDeltaTime;
            }
        }

        private bool CanAttack()
        {
            // 检查是否在攻击冷却中
            if (attackCooldownTimer > 0f)
            {
                return false;
            }
            attackCooldownTimer = attackCooldown;
            // 检查Boss与玩家之间的距离是否小于瞄准距离
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            return distanceToPlayer <= aimDistance && distanceToPlayer >= atkDistance;
        }

        private void AimAtPlayer()
        {
            // 计算朝向玩家的方向
            Vector3 directionToPlayer = playerTransform.position - transform.position;
            directionToPlayer.y = 0f;
            directionToPlayer.Normalize();

            // 旋转Boss以瞄准玩家
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime);
        }

        private void ShootProjectile()
        {
            // 生成远程投射物
            if (projectilePrefab != null && projectileSpawnPoint != null)
            {
                // GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
                var projectile = _throwingsPool.Get();
            }
        }
    }
}
