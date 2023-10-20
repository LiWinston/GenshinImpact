using System.Collections;
using Behavior.Health;
using UnityEngine;
using UnityEngine.Pool;
using Utility;

namespace Behavior.Skills
{
    public class MonsterProjectile : MonoBehaviour, Utility.IPoolable
    {
        [SerializeField] private float rotateSpeed = 30f;
        public float projectileSpeed = 10f; // 投射物速度

        private Transform _target; // 玩家对象的引用
        private IDamageable _damageable;
        // private Vector3 initialPosition; // 投射物初始位置
        private bool _hasHit = false;
        public MonsterBehaviour _monsterBehaviour;
        private Coroutine existCoroutine;
        [SerializeField] private float maxExistTime = 5f;
        private float dmg;


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
            
            if (_monsterBehaviour.target.layer == LayerMask.NameToLayer("Player"))
            {
                _target = Find.FindDeepChild(PlayerController.Instance.transform, "neck_01"); // 获取玩家对象
                _damageable = PlayerController.Instance;
                dmg = _monsterBehaviour.monsterLevel/20 *Random.Range(_monsterBehaviour.minAttackPower, _monsterBehaviour.maxAttackPower) *
                      (_monsterBehaviour.isBoss ? 1 : Random.Range(0.1f, 0.5f));//双标对待玩家和同类
            }
            else
            {
                _target = Find.FindDeepChild(_monsterBehaviour.target.transform, "head"); 
                _damageable = _monsterBehaviour.target.GetComponent<IDamageable>();
            }
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
            
            
        }

        private void Update()
        {
            // if(GetComponent<Rigidbody>().velocity.magnitude < 0.01f) ThisPool.Release(gameObject);
            if (!_hasHit && IsExisting)
            {
                if (_target != null)
                {
                    var distance = _target.position - transform.position;
                    if(distance.magnitude < 0.25f) HitTarget();
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
    
        private void HitTarget()
        {
            
            _damageable.TakeDamage(dmg);
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