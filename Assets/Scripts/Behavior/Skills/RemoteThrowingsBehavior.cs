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
    internal float triggerRange = 0.5f;
    internal float damage = 500f;
    internal float AOEDamage = 100f;
    [SerializeField]internal float AOERange = 1.5f;
    [SerializeField] internal PositionalCategory positionalCategory;
    [SerializeField] internal EffectCategory _effectCategory = EffectCategory.Explosion;
    public float maxExistTime = 10f;
    // private bool hasEnemyInside = false; // 标志是否有敌人在碰撞内
    private bool detectedEnemy = false; // 标志是否已经检测到敌人
    private GameObject target = null;
    private int bounceCount = 0;
    private bool hasAppliedFirstDamage = false;
    private bool hasAppliedAOE;
    private HashSet<Collider> hitEnemies;
    
    [InspectorLabel("Player")]
    [SerializeField]internal float _energyCost;

    private Coroutine existCoroutine;
    private int enemyLayer;
    

    private void Start(){
        hitEnemies = new HashSet<Collider>();
    }

    private void Awake(){
        enemyLayer = LayerMask.NameToLayer("Enemy");
    }


    public void actionOnGet(){
        if(_effectCategory == EffectCategory.Existing){
            existCoroutine = StartCoroutine(StartExistenceTimer());
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
        target = null;
    }

    public void Release()
    {
        ThisPool.Release(gameObject);
    }
    
   
    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log(other.name+"Enter Trigger,层级 " + other.gameObject.layer.ToString() + "检测层级" + enemyLayer.ToString());
        if (other.gameObject.layer == enemyLayer && !hitEnemies.Contains(other))
        {
            
            switch (_effectCategory)
            {
                case EffectCategory.Bouncing:
                case EffectCategory.Explosion:
                {
                    if (!hasAppliedFirstDamage)
                    {
                        ApplyEffect(other);
                        Debug.Log(other.name+"是本轮唯一首次攻击");
                    }
                    hasAppliedFirstDamage = true;
                    break;
                }
                case EffectCategory.Existing:
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
            hitEnemies.Add(other); // 记录已攻击过的敌人
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
            StartCoroutine(ApplyDamageOverTime(other));
        }
        else if (_effectCategory == EffectCategory.Bouncing)
        {
            if(other.gameObject.layer == enemyLayer) ApplyBouncingDamage(other.gameObject);
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
        Debug.Log("AOE!");
        var colliders = Physics.OverlapSphere(transform.position, AOERange, LayerMask.GetMask("Enemy"));
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
        var mstbhv = other.GetComponent<MonsterBehaviour>();
        if (mstbhv != null)
        {
            var dmg = damage * Mathf.Pow(0.8f, bounceCount);
            Debug.Log("击中" + other.name + "dmg = "+ dmg);
            mstbhv.TakeDamage(dmg);
            hitEnemies.Add(mstbhv.GetComponent<Collider>());
            bounceCount++;
            if (bounceCount > 10 || Mathf.Pow(0.8f, bounceCount) < 0.06f)
            {
                Release();
            }
        }
        var nextTarget = GetBounceTarget();
        while(hitEnemies.Contains(nextTarget.GetComponent<Collider>()))
        {
            nextTarget = GetBounceTarget();
        }
        if (nextTarget != null)
        {
            target = nextTarget;
            transform.LookAt(target.transform);
            StartCoroutine(Bounce());
        }
        else Release();
    }

    private IEnumerator Bounce()
    {
        float duration = 0.4f; // 跳跃的总时间
        float startTime = Time.time;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = target.transform.position;

        Debug.Log("在跳了");
        // while (Time.time - startTime < duration)
        // {
        //     float t = (Time.time - startTime) / duration;
        //     // 使用插值将物体从起始位置移动到目标位置
        //     transform.position = Vector3.Lerp(startPosition, endPosition, t);
        //     yield return null;
        // }
        // 确保物体准确到达目标位置
        yield return new WaitForSeconds(1);
        transform.position = endPosition;
        
        // 调用ApplyBouncingDamage来处理新的目标
        ApplyBouncingDamage(target.gameObject);
    }
    private IEnumerator StartExistenceTimer()
    {
        yield return new WaitForSeconds(maxExistTime);
        Release();
    }
    
    private GameObject GetBounceTarget()
    {
        Collider[] nearEnemies = Physics.OverlapSphere(target.transform.position, AOERange, enemyLayer);
        List<GameObject> validEnemies = new List<GameObject>();
        
        foreach (Collider enemyCollider in nearEnemies)
        {
            if (enemyCollider.gameObject == gameObject) continue;
            MonsterBehaviour enemyMonster = enemyCollider.GetComponent<MonsterBehaviour>();
            if (!hitEnemies.Contains(enemyMonster.GetComponent<Collider>()) && enemyMonster != null && !enemyMonster.health.IsDead())
            {
                validEnemies.Add(enemyCollider.gameObject);
            }
        }

        if (validEnemies.Count > 0)
        {
            // 随机选择一个有效敌人作为目标
            int randomIndex = Random.Range(0, validEnemies.Count - 1);
            return validEnemies[randomIndex];
        }
        else
        {
            return target == null ? target : gameObject;
        }
    }

}