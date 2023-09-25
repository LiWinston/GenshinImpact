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
    private HashSet<Collider> hitEnemies;
    
    [InspectorLabel("Player")]
    [SerializeField]internal float _energyCost;

    private Coroutine existCoroutine;
    private int enemyLayer;
    
    private void Awake(){
            enemyLayer = LayerMask.NameToLayer("Enemy");
        }

    private void Start(){
        hitEnemies = new HashSet<Collider>();
    }

    public void Update(){
       // Debug.Log(hasAppliedFirstDamage);
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
        if (other.gameObject.layer == enemyLayer && !hitEnemies.Contains(other))
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
            hitEnemies.Add(mst.GetComponent<Collider>());
            bounceCount++;
            // if (bounceCount > 10 || Mathf.Pow(0.8f, bounceCount) < 0.06f)
            if (Mathf.Pow(0.8f, bounceCount) < 0.06f) Release();
        }
        GameObject nextTarget = GetBounceTarget();
        Debug.Log("择取下一个："+nextTarget);
        while(hitEnemies.Contains(nextTarget.GetComponent<Collider>()))
        {
            Debug.Log("不合适，再换："+nextTarget);
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
        yield return new WaitForSeconds(duration);
        transform.position = endPosition;
        
        // 调用ApplyBouncingDamage来处理新的目标
        ApplyBouncingDamage(target.gameObject);
    }
    private IEnumerator StartExistenceTimer()
    {
        yield return new WaitForSeconds(maxExistTime);
        Release();
    }
    
    private GameObject GetBounceTarget(){
        Debug.Log("GetBounceTarget调用");
        Collider[] nearEnemies = Physics.OverlapSphere(target.transform.position, AOERange, LayerMask.GetMask("Enemy"));
        Debug.Log("nearEnemies长度"+nearEnemies.Length);
        List<GameObject> validEnemies = new List<GameObject>();
        
        foreach (Collider enemyCollider in nearEnemies)
        {
            if (enemyCollider.gameObject == gameObject) continue;
            MonsterBehaviour enemyMonster = enemyCollider.GetComponent<MonsterBehaviour>();
            if (!hitEnemies.Contains(enemyMonster.GetComponent<Collider>()) && enemyMonster != null && !enemyMonster.health.IsDead())
            {
                Debug.Log("如果敌人非空没有被攻击过且不是死亡状态");
                validEnemies.Add(enemyCollider.gameObject);
            }
        }
        Debug.Log("validEnemies长度"+validEnemies.Count);
        if (validEnemies.Count > 0)
        {
            // 随机选择一个有效敌人作为目标
            int randomIndex = Random.Range(0, validEnemies.Count - 1);
            return validEnemies[0];
        }
        else
        {
            return target == null ? target : gameObject;
        }
    }

}