using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using enemyBehaviour;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

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
    
    
    [InspectorLabel("Effect")]
    [SerializeField] internal float triggerRange = 0.5f;
    [SerializeField] internal float damage = 500;
    [SerializeField] internal float AOEDamage = 300f;
    [SerializeField] internal float AOERange = 1.5f;
    [SerializeField] internal PositionalCategory positionalCategory;
    [SerializeField] internal EffectCategory _effectCategory = EffectCategory.Explosion;
    [SerializeField] public float maxExistTime = 10f;
    // private bool hasEnemyInside = false; // 标志是否有敌人在碰撞内
    private bool detectedEnemy = false; // 标志是否已经检测到敌人
    private GameObject target = null;
    private int bounceCount = 0;
    private bool hasAppliedFirstDamage = false;
    private bool hasAppliedAOE;
    private HashSet<GameObject> hitEnemies;
    
    [InspectorLabel("Player")]
    [SerializeField]internal float _energyCost;

    private Coroutine existCoroutine;
    private Coroutine DamageOverTimeCoroutine_Existing;
    private int enemyLayer;
    
    private void Awake(){
            enemyLayer = LayerMask.NameToLayer("Enemy");
        }

    private void Start(){
        hitEnemies = new HashSet<GameObject>();
    }

    public void Update(){
       // Debug.Log(hasAppliedFirstDamage);
    }

    public void actionOnGet(){
        if(_effectCategory == EffectCategory.Existing){
            existCoroutine = StartCoroutine(ReturnToPoolDelayed(maxExistTime));
        }
        // hasEnemyInside = false;
        detectedEnemy = false;
        bounceCount = 0;
        IsExisting = true;
        hasAppliedAOE = false;
        hasAppliedFirstDamage = false;
    }

    public void actionOnRelease(){
        hitEnemies.Clear();
        if(existCoroutine != null){
            StopCoroutine(existCoroutine);
        }
        if(DamageOverTimeCoroutine_Existing != null){
            StopCoroutine(DamageOverTimeCoroutine_Existing);
        }
        IsExisting = false;
        target = null;
    }

    public void Release()
    {
        ThisPool.Release(gameObject);
    }
    
   
    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log(other.name+"Enter Trigger,层级 " + other.gameObject.layer.ToString() + "检测层级" + enemyLayer.ToString());
        if (other.gameObject.layer == enemyLayer && !hitEnemies.Contains(other.gameObject))
        {
            
            switch (_effectCategory)
            {
                case EffectCategory.Bouncing:
                case EffectCategory.Explosion:
                {
                    Debug.Log(other.name+"进入前俩分支");
                    if (!hasAppliedFirstDamage)
                    {
                        hasAppliedFirstDamage = true;
                        Debug.Log(other.name+"是本轮唯一首次攻击");
                        ApplyEffect(other);
                    }
                    break;
                }
                case EffectCategory.Existing:
                    Debug.Log(other.name+"第三分支 正常触发伤害");
                    ApplyEffect(other);
                    break;
            }
            
            if (_effectCategory != EffectCategory.Bouncing && !hasAppliedAOE)
            {
                ApplyAOEEffect();
            }
            hasAppliedAOE = true;
            
            // hasEnemyInside = true;
            detectedEnemy = true;
            hitEnemies.Add(other.gameObject); // 记录已攻击过的敌人
        }

        if (positionalCategory == PositionalCategory.Throwing && other.gameObject.layer == LayerMask.NameToLayer("Wall") && !detectedEnemy)
        {
            ApplyAOEEffect();
            Release();
        }
    }
    
    private void OnTriggerExit(Collider other)//TODO：真的能触发吗 有待测试 other是墙又不会跑出去 
    {
        // Debug.Log(other.name+"Exit Trigger");
        if (positionalCategory == PositionalCategory.Throwing && other.gameObject.layer == LayerMask.NameToLayer("Wall") && !detectedEnemy)
        {
            ApplyAOEEffect();
            hasAppliedAOE = true;
            Release();
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
            target = other.gameObject;//尼玛，终于找到罪魁祸首了
            ApplyBouncingDamage(other.gameObject);
        }
        else if (_effectCategory == EffectCategory.Explosion)
        {
            var mstbhv = other.GetComponent<MonsterBehaviour>();
            if (mstbhv != null)
            {
                mstbhv.TakeDamage(damage);
            }
        }
    }

    private void ApplyAOEEffect()
    {
        var colliders = Physics.OverlapSphere(transform.position, AOERange, LayerMask.GetMask("Enemy"));
        Debug.Log("AOE人数" + colliders.Length);
        foreach (var collider in colliders)
        {
            var mstbhv = collider.GetComponent<MonsterBehaviour>();
            if (mstbhv != null)
            {
                mstbhv.TakeDamage(AOEDamage);
            }
        }
    }
    
    private IEnumerator ApplyDamageOverTime(Collider other)
    {
        var mstbhv = other.GetComponent<MonsterBehaviour>();
        if (mstbhv != null)
        {
            float duration = maxExistTime;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                mstbhv.TakeDamage(AOEDamage * Time.deltaTime);
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
    }

    private void ApplyBouncingDamage(GameObject other)
    {
        var mst = other.GetComponent<MonsterBehaviour>();
        if (mst)
        {
            var dmg = damage * Mathf.Pow(0.8f, bounceCount);
            Debug.Log("击中" + other.name + "dmg = "+ dmg);
            mst.TakeDamage(dmg);
            hitEnemies.Add(mst.gameObject);
            bounceCount++;
            // if (bounceCount > 10 || Mathf.Pow(0.8f, bounceCount) < 0.06f)
            if (Mathf.Pow(0.8f, bounceCount) < 0.06f) Release();
        }
        GameObject nextTarget = GetBounceTarget();
        if (nextTarget != target && nextTarget != null)
        {
            target = nextTarget;
            transform.LookAt(target.transform);
            StartCoroutine(Bounce());
            Debug.Log("择取下一个："+nextTarget);
        }else Release();
    }

    private IEnumerator Bounce()
    {
        float duration = 0.3f; // 跳跃的总时间
        float startTime = Time.time;

        // Debug.Log("在跳了");
        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            // 使用插值将物体从起始位置移动到目标位置
            transform.position = Vector3.Lerp(transform.position, target.transform.position, t);
            yield return null;
        }
        yield return new WaitForSeconds(duration);
        transform.position = target.transform.position;
        
        // 调用ApplyBouncingDamage来处理新的目标
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
            // 随机选择一个有效敌人作为目标
            // int randomIndex = Random.Range(0, validEnemies.Count - 1);
            return validEnemies[0];
        }

        return target;
    }

    private IEnumerator ReturnToPoolDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        // Return the object to the object pool
        if (gameObject.activeSelf)
        {
            Release();
        }
    }
}