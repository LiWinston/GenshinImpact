using UnityEngine;

namespace Behavior.Skills
{
    public class MonsterProjectile : MonoBehaviour
    {
        [SerializeField] private float rotateSpeed = 30f;
        public float projectileSpeed = 10f; // 投射物速度
        public float damage = 10f; // 投射物伤害

        private Transform _target; // 玩家对象的引用
        // private Vector3 initialPosition; // 投射物初始位置
        private bool _hasHit = false;
        public MonsterBehaviour _monsterBehaviour;
        

        private void Start()
        {
            // initialPosition = transform.position;
            _target = PlayerController.Instance.pickHandTransform; // 获取玩家对象
        }

        private void Update()
        {
            if (!_hasHit)
            {
                if (_target != null)
                {
                    var distance = _target.position - transform.position;
                    if(distance.magnitude < 0.5f) HitPlayer();
                    // 计算朝向玩家的方向
                    var direction = distance.normalized;

                    // 使用球形插值来平滑调整投射物方向
                    Quaternion rotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotateSpeed * Time.deltaTime);

                    // 让投射物向前移动
                    transform.Translate(Vector3.forward * (projectileSpeed * Time.fixedDeltaTime));
                }
                else
                {
                    // 如果目标丢失，销毁投射物
                    Destroy(gameObject);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // // if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
            // if(other.gameObject.CompareTag("Player"))
            // {
            //     Debug.Log("Hit Player");
            //     HitPlayer();
            // }
            if (other.gameObject.layer == LayerMask.GetMask("Wall", "Floor")) Destroy(this.gameObject);
            if (other.gameObject.layer== LayerMask.NameToLayer("Wall") || other.gameObject.layer == LayerMask.NameToLayer("Floor"))
            {
                Debug.Log("Hit Wall");
                Destroy(this.gameObject);
            }
        }
    
        private void HitPlayer()
        {
            // 玩家受到伤害
        
            PlayerController.Instance.TakeDamage(_monsterBehaviour.minAttackPower * Random.Range(1f, 1.5f));
            
        

            // 标记为已击中，以避免重复伤害
            _hasHit = true;

            // 在此可以播放攻击效果或销毁投射物
            // 例如，你可以播放粒子效果来表示怪物攻击击中了玩家

            // 销毁投射物
            Destroy(gameObject);
        }
    }
}