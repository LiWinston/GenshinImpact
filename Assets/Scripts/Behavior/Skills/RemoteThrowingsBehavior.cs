using System;
using System.Collections;
using System.Collections.Generic;
using enemyBehaviour;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

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
    internal float triggerRange = 1f;
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
    private bool hasAppliedAOE;
    private HashSet<Collider> hitEnemies = new HashSet<Collider>();
    
    [InspectorLabel("Player")]
    [SerializeField]internal float _energyCost;

    private Coroutine existCoroutine;
    private int enemyLayer;

    private void Awake(){
        enemyLayer = LayerMask.GetMask("Enemy");
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
    }

    public void actionOnRelease(){
        hitEnemies.Clear();
        if(existCoroutine != null){
            StopCoroutine(existCoroutine);
        }
    }

    public void Release()
    {
        ThisPool.Release(gameObject);
        target = null;
    }
    
   
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.GetMask("Enemy") && !hitEnemies.Contains(other))
        {
            ApplyEffect(other);
            if (!hasAppliedAOE)
            {
                ApplyAOEEffect();
            }
            hasAppliedAOE = true;
            
            // hasEnemyInside = true;
            detectedEnemy = true;
            hitEnemies.Add(other); // 记录已攻击过的敌人
        }

        if (positionalCategory == PositionalCategory.Throwing && other.gameObject.layer == LayerMask.GetMask("Wall") && !detectedEnemy)
        {
            ApplyAOEEffect();
            Release();
        }
    }
    
    private void OnTriggerExit(Collider other)//TODO：真的能触发吗 有待测试 other是墙又不会跑出去 
    {
        if (positionalCategory == PositionalCategory.Throwing && other.gameObject.layer == LayerMask.GetMask("Wall") && !detectedEnemy)
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
            // Check if it's an existing effect, apply damage over time
            StartCoroutine(ApplyDamageOverTime(other));
        }
        else if (_effectCategory == EffectCategory.Bouncing)
        {
            // Check if it's a bouncing effect, apply damage with bounce
            ApplyBouncingDamage(other.gameObject);
        }
        else
        {
            // Check if it's an explosion or immediate effect, apply primary damage
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
            mstbhv.TakeDamage(damage * Mathf.Pow(0.8f, bounceCount));
            bounceCount++;
            if (bounceCount > 5 || Mathf.Pow(0.8f, bounceCount) < 0.1f)
            {
                Release();
            }
        }
        var nextTarget = GetBounceTarget();
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
        float duration = 0.5f; // 跳跃的总时间
        float startTime = Time.time;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = target.transform.position;

        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            // 使用插值将物体从起始位置移动到目标位置
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
            yield return null;
        }

        // 确保物体准确到达目标位置
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
            if (target.layer == enemyLayer && !target.GetComponent<MonsterBehaviour>().health.IsDead())
            {
                // 如果目标是存活的敌人，返回目标
                return target;
            }
            Collider[] nearEnemies = Physics.OverlapSphere(transform.position, AOERange, enemyLayer);
            GameObject nearestEnemy = null;
            float nearestDistance = float.MaxValue;
            foreach (Collider enemyCollider in nearEnemies)
            {
                if (enemyCollider.gameObject == gameObject) continue;
                // 检查敌人是否存活
                MonsterBehaviour enemyMonster = enemyCollider.GetComponent<MonsterBehaviour>();
                if (enemyMonster != null && !enemyMonster.health.IsDead())
                {
                    float distance = Vector3.Distance(transform.position, enemyCollider.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestEnemy = enemyCollider.gameObject;
                        nearestDistance = distance;
                    }
                }
            }

            // curDistance = nearestDistance;
            return nearestEnemy != null ? nearestEnemy : gameObject;// 如果没有找到最近的敌人，返回当前目标
        }
}