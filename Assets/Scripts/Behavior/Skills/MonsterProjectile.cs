using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using Utility;

namespace Behavior.Skills
{
    public class MonsterProjectile : MonoBehaviour, Utility.IPoolable
    {
        [SerializeField] private float rotateSpeed = 30f;
        public float projectileSpeed = 10f; // 投射物速度
        public float damage = 10f; // 投射物伤害

        private Transform _target; // 玩家对象的引用
        // private Vector3 initialPosition; // 投射物初始位置
        private bool _hasHit = false;
        public MonsterBehaviour _monsterBehaviour;
        private Coroutine existCoroutine;
        [SerializeField] private float maxExistTime = 5f;


        public ObjectPool<GameObject> ThisPool { get; set; }
        public bool IsExisting { get; set; }
        

        public void actionOnGet()
        {
            _hasHit = false;
            gameObject.SetActive(true);
            // GetComponent<Rigidbody>().velocity = _monsterBehaviour.transform.forward * projectileSpeed;
            existCoroutine = StartCoroutine(ReturnToPoolDelayed(maxExistTime));
            IsExisting = true;
            GetComponent<AudioSource>().Play();
        }

        public void actionOnRelease()
        {
            // GetComponent<Rigidbody>().velocity = Vector3.zero;
            _hasHit = false;
            if(existCoroutine != null){
                StopCoroutine(existCoroutine);
                existCoroutine = null;
            }
            IsExisting = false;
            GetComponent<AudioSource>().Stop();
        }
        
        private void Start()
        {
            // initialPosition = transform.position;
            _target = Find.FindDeepChild(PlayerController.Instance.transform, "neck_01"); // 获取玩家对象
        }

        private void Update()
        {
            // if(GetComponent<Rigidbody>().velocity.magnitude < 0.01f) ThisPool.Release(gameObject);
            if (!_hasHit && IsExisting)
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
                    ThisPool.Release(gameObject);
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
                ThisPool.Release(gameObject);
            }
        }
    
        private void HitPlayer()
        {
            // 玩家受到伤害
            PlayerController.Instance.TakeDamage(_monsterBehaviour.monsterLevel/20 *Random.Range(_monsterBehaviour.minAttackPower, _monsterBehaviour.maxAttackPower));
            
        

            // 标记为已击中，以避免重复伤害
            _hasHit = true;

            // 在此可以播放攻击效果或销毁投射物
            // 例如，你可以播放粒子效果来表示怪物攻击击中了玩家

            // 销毁投射物
            ThisPool.Release(gameObject);
        }
        
        private IEnumerator ReturnToPoolDelayed(float delay)
        {
            yield return new WaitForSeconds(delay);
            // Return the object to the object pool
            if (IsExisting)
            {
                ThisPool.Release(gameObject);
            }
        }
    }
}