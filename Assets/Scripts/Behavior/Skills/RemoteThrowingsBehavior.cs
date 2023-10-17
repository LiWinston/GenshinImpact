using System.Collections;
using System.Collections.Generic;
using Behavior.Effect;
using Game;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using IPoolable = Utility.IPoolable;
using Random = UnityEngine.Random;

namespace Behavior.Skills
{
    [System.Serializable]
    public class RemoteThrowingsBehavior : MonoBehaviour, IPoolable
    {
        internal enum EffectCategory
        {
            Explosion,
            Existing,
            Bouncing
        }
        internal enum PositionalCategory
        {
            Throwing,
            ImmediatelyInPosition
        }
    
        public ObjectPool<GameObject> ThisPool { get; set; }
        public bool IsExisting { get; set; }
        private Coroutine existCoroutine;
    
        [InspectorLabel("Effect")]
        [SerializeField] internal float triggerRange = 0.5f;
        [SerializeField] internal float damage = 500;
        [SerializeField] internal float AOEDamage = 300f;
        //若技能释放处勾选了随次数变强，此处的伤害值将会被忽略，而使用按表求得的伤害值
        //If the RemoteSpelling bool is set to increase with number of times,
        //the damage value here will be ignored and the damage value calculated according to the table will be used
        [SerializeField] internal float AOERange = 1.5f;
        [SerializeField] internal PositionalCategory positionalCategory;
        [SerializeField] internal EffectCategory _effectCategory = EffectCategory.Explosion;
        [SerializeField] public float maxExistTime = 10f;
        // private bool hasEnemyInside = false; // 标志是否有敌人在碰撞内
        private bool detectedEnemy = false; // Flag whether an enemy has been detected
        private GameObject target = null;
        private int bounceCount = 0;
        private int BouncingEXP = 0;
        private bool hasAppliedFirstDamage = false;
        private bool hasAppliedAOE;
        private HashSet<GameObject> hitEnemies;
        private Coroutine DamageOverTimeCoroutine_Existing;
    
        [InspectorLabel("Player")]
        [SerializeField]internal float _energyCost;
        private int enemyLayer;

        [InspectorLabel("ThrowingAttributes")]
        public float throwingSpeed = 3f;
    
        [InspectorLabel("Sound Customization -- 音效自定义")]
        [SerializeField] public AudioClip startAudioClip;
        [SerializeField] public AudioClip hitAudioClip;
        private EffectTimeManager _effectTimeManager;
        

        private void Awake(){
            _effectTimeManager = GetComponent<EffectTimeManager>();
            enemyLayer = LayerMask.NameToLayer("Enemy");
        }

        private void Start(){
            hitEnemies = new HashSet<GameObject>();
        }

        public void Update(){
            // Debug.Log(hasAppliedFirstDamage);
        }

        public void actionOnGet(){
            gameObject.SetActive(true);
            existCoroutine = StartCoroutine(ReturnToPoolDelayed(maxExistTime));
            // hitEnemies.Clear();
            // hasEnemyInside = false;
            detectedEnemy = false;
            bounceCount = 0;
            BouncingEXP = 0;
            IsExisting = true;
            hasAppliedAOE = false;
            hasAppliedFirstDamage = false;
        }

        public void actionOnRelease(){
            hitEnemies.Clear();
            if(DamageOverTimeCoroutine_Existing != null){
                StopCoroutine(DamageOverTimeCoroutine_Existing);
            }
            if(existCoroutine != null){
                StopCoroutine(existCoroutine);
                existCoroutine = null;
            }
            if(BounceCoroutine != null){
                _effectTimeManager.StopEffect("Bounce");
                StopCoroutine(BounceCoroutine);
                BounceCoroutine = null;
            }
            if(_effectCategory == EffectCategory.Bouncing && !GameManager.Instance.IsFinalBattle){
                PlayerController.Instance.ShowPlayerHUD("Extract EXP + " + BouncingEXP);
                PlayerController.Instance.state.AddExperience(BouncingEXP);
            }
            IsExisting = false;
            target = null;
            // StartCoroutine(checkRelease());
            GetComponent<AudioSource>().Stop();
        }

        // private void OnBecameInvisible()
        // {
        //     if(positionalCategory == PositionalCategory.Throwing) ThisPool.Release(gameObject);
        // }
        

        private void OnTriggerEnter(Collider other)
        {
            // Debug.Log(other.name+"Enter Trigger,level " + other.gameObject.layer.ToString() + "Detection level" + enemyLayer.ToString());
            if (other.gameObject.layer == enemyLayer && !hitEnemies.Contains(other.gameObject))
            {
            
                switch (_effectCategory)
                {
                    case EffectCategory.Bouncing:
                    case EffectCategory.Explosion:
                    {
                        // Debug.Log(other.name+"Enter the first two branches");
                        if (!hasAppliedFirstDamage)
                        {
                            hasAppliedFirstDamage = true;
                            // Debug.Log(other.name+"This is the only first attack in this round");
                            ApplyEffect(other);
                        }
                        break;
                    }
                    case EffectCategory.Existing:
                        // Debug.Log(other.name+"The third branch triggers damage normally");
                        ApplyEffect(other);
                        break;
                }
            
                if (_effectCategory != EffectCategory.Bouncing && !hasAppliedAOE)
                {
                    if(AOEDamage > 0) ApplyAOEEffect();
                    // if(_effectCategory != EffectCategory.Existing) ThisPool.Release(gameObject);//This line will cause self to disappear if they hit target directly.
                    hasAppliedAOE = true;
                    if (_effectCategory == EffectCategory.Explosion)
                    {
                        ThisPool.Release(gameObject);
                    }
                }
            
                // hasEnemyInside = true;
                detectedEnemy = true;
                hitEnemies.Add(other.gameObject); // Record enemies that have been attacked
            }

            if (positionalCategory == PositionalCategory.Throwing && other.gameObject.layer == LayerMask.NameToLayer("Wall") )
            {
                if (_effectCategory == EffectCategory.Explosion)
                {
                    ApplyAOEEffect();
                    ThisPool.Release(gameObject);
                }
                else if (_effectCategory == EffectCategory.Bouncing)
                {
                    if(!GetBounceTarget()) ThisPool.Release(gameObject);
                }
            }
        }
    
        private void OnTriggerExit(Collider other)//TODO：Can it really be triggered? needs to be tested. --other is a wall which will not escape.
        {
            // Debug.Log(other.name+"Exit Trigger");
            if (positionalCategory == PositionalCategory.Throwing && other.gameObject.layer == LayerMask.NameToLayer("Wall") && !detectedEnemy)
            {
                ApplyAOEEffect();
                hasAppliedAOE = true;
                ThisPool.Release(gameObject);
            }
        }
    
        private void ApplyEffect(Collider other)
        {
            if (_effectCategory == EffectCategory.Existing)
            {
                DamageOverTimeCoroutine_Existing = StartCoroutine(ApplyDamageOverTime(other));
            }
            else if (_effectCategory == EffectCategory.Bouncing)
            {
                target = other.gameObject;//Damn, finally found the culprit.
                transform.position = target.transform.position + Vector3.up * Random.Range(0.3f,1.2f);
                ApplyBouncingDamage(other.gameObject);
            }
            else if (_effectCategory == EffectCategory.Explosion)
            {
                var mstbhv = other.GetComponent<MonsterBehaviour>();
                if (mstbhv == null) return;
                if(hitAudioClip) SoundEffectManager.Instance.PlaySound(hitAudioClip, mstbhv.gameObject);
                mstbhv.TakeDamage(damage);
                // Debug.Log("Hit" + other.name + "dmg = "+ damage);
                // ThisPool.Release(gameObject);
            }
        }

        private void ApplyAOEEffect()
        {
            var colliders = Physics.OverlapSphere(transform.position, AOERange, LayerMask.GetMask("Enemy"));
            // Debug.Log("AOE involved counter:" + colliders.Length);
            foreach (var collider in colliders)
            {
                var mstbhv = collider.GetComponent<MonsterBehaviour>();
                if (mstbhv != null)
                {
                    mstbhv.TakeDamage(AOEDamage);
                    Debug.Log("ApplyAOEDamage" + AOEDamage);
                }
            }
        }
    
        private IEnumerator ApplyDamageOverTime(Collider other)
        {
            var mstbhv = other.GetComponent<MonsterBehaviour>();
            if (mstbhv != null)
            {
                Debug.Log("ApplyDamageOverTime" + damage);
                float duration = maxExistTime;
                int id = GetInstanceID();
                mstbhv._effectTimeManager.CreateEffectBar("Burn" + id, Color.red, duration);
                float elapsed = 0f;
                while (elapsed < duration)
                {
                    mstbhv.TakeDamage(AOEDamage * Time.deltaTime);
                    elapsed += Time.deltaTime;
                    yield return null;
                    if (mstbhv.health.IsDead())
                    {
                        mstbhv._effectTimeManager.StopEffect("Burn" + id);
                        break;
                    }
                }
                mstbhv._effectTimeManager.StopEffect("Burn" + id);
            }
        }

        private void ApplyBouncingDamage(GameObject other)
        {
            var mst = other.GetComponent<MonsterBehaviour>();
            if (mst)
            {
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                var dmg = damage * Mathf.Pow(0.8f, bounceCount);
                // Debug.Log("Hit" + other.name + "dmg = "+ dmg);
                mst.TakeDamage(dmg);
                //加强 ： 弹人能升级
                // PlayerController.Instance.state.AddExperience(mst.monsterExperience);
                // PlayerController.Instance.ShowPlayerHUD("Bounce EXP + " + mst.monsterExperience);
                BouncingEXP += mst.monsterExperience;
                
                if(hitAudioClip) SoundEffectManager.Instance.PlaySound(hitAudioClip, gameObject);
                hitEnemies.Add(mst.gameObject);
                bounceCount++;
                // if (bounceCount > 10 || Mathf.Pow(0.8f, bounceCount) < 0.06f)
                if (Mathf.Pow(0.8f, bounceCount) < 0.06f)
                {
                    ThisPool.Release(gameObject);
                    _effectTimeManager.StopEffect("Bounce");
                }
            }
            GameObject nextTarget = GetBounceTarget();
            if (nextTarget != target && nextTarget != null)
            {
                //球子第一次加计时条
                if(bounceCount == 1) _effectTimeManager.CreateEffectBar("Bounce", Color.magenta, 3f);
                target = nextTarget;
                transform.LookAt(target.transform);
                BounceCoroutine = StartCoroutine(Bounce());
                // Debug.Log("择取下一个："+nextTarget);
            }
            else
            {
                ThisPool.Release(gameObject);
            }
        }

        public Coroutine BounceCoroutine { get; set; }
        
        

        private IEnumerator Bounce()
        {
            float duration = 0.25f; // total Bounce time
            float startTime = Time.time;
            // Debug.Log("Bouncing");
            while (Time.time - startTime < duration)
            {
                float t = (Time.time - startTime) / duration;
                // Move from starting position to target position using interpolation
                transform.position = Vector3.Lerp(transform.position, target.transform.position + Vector3.up * Random.Range(0.3f,1.2f), t);
                yield return null;
            }
            yield return new WaitForSeconds(duration);
            transform.position = target.transform.position;
        
            // Call ApplyBouncingDamage to handle the new target
            ApplyBouncingDamage(target.gameObject);
        }
  
    
        private GameObject GetBounceTarget(){
            // Debug.Log("GetBounceTarget调用");
            Collider[] nearEnemies = Physics.OverlapSphere(target.transform.position, AOERange, LayerMask.GetMask("Enemy"));
            // Debug.Log("nearEnemies长度"+nearEnemies.Length);
            List<GameObject> validEnemies = new List<GameObject>();
        
            foreach (Collider enemyCollider in nearEnemies)
            {
                if (enemyCollider.gameObject.transform == transform) continue;
                MonsterBehaviour enemyMonster = enemyCollider.GetComponent<MonsterBehaviour>();
                if (!hitEnemies.Contains(enemyMonster.gameObject) && enemyMonster != null && !enemyMonster.health.IsDead())
                {
                    // Debug.Log("如果敌人非空没有被攻击过且不是死亡状态");
                    validEnemies.Add(enemyCollider.gameObject);
                }
            }
            // Debug.Log("validEnemies长度"+validEnemies.Count);
            if (validEnemies.Count > 0)
            {
                // Randomly selects a valid enemy as the target
                // int randomIndex = Random.Range(0, validEnemies.Count - 1);
                return validEnemies[Random.Range(0, validEnemies.Count - 1)];
            }

            return target;
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