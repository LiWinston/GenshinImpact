using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

[System.Serializable]
public class PrefabGenerator : MonoBehaviour
{
    public GameObject prefab;
    [SerializeField] private float initialSpawnInterval = 1f;
    [SerializeField] private float minSpawnInterval = 0.1f;
    [SerializeField] private float spawnHeight = 1f;
    [SerializeField] private float accelerationRate = 0.1f;
    [SerializeField] private float maxExistTime = 10f;

    private float spawnTimer = 0f;
    private float spawnInterval;
    private Vector3 targetPosition;
    
    private ObjectPool<GameObject> objPool;
    public int countAll;
    public int countActive;
    public int countInactive;
    
    private void Start()
    {
        targetPosition = transform.position;
        spawnInterval = initialSpawnInterval;
        objPool = new ObjectPool<GameObject>(CreateFunc, actionOnGet, actionOnRelease, actionOnDestroy,
            true, 40, 200);
    }
    GameObject CreateFunc()
    {
        Vector3 spawnPosition = targetPosition + new Vector3(Random.Range(-10f, 10f), spawnHeight, Random.Range(-10f, 10f));
        var prfb = Instantiate(prefab, spawnPosition, Quaternion.identity);
        prfb.GetComponent<IPoolable>().SetPool(objPool);
        return prfb;
    }

    private void SetPoolForGeneratedObject(GameObject generatedObject)
    {
        // 检查生成的对象的类型
        if (generatedObject.GetComponent<MonsterBehaviour>() != null)
        {
            generatedObject.GetComponent<MonsterBehaviour>().pool = objPool;
        }
        else if (generatedObject.GetComponent<Pickable>() != null)
        {
            generatedObject.GetComponent<Pickable>().pool = objPool;
        }
        // More to generate and set Pool
    }
    
    void actionOnGet(GameObject obj)
    {
        obj.SetActive(true);
    }

    void actionOnRelease(GameObject obj)
    {
        obj.SetActive(false);
    }

    void actionOnDestroy(GameObject obj)
    {
        Destroy(obj);
    }
    
    

    private void Update()
    {
        countAll = objPool.CountAll;
        countActive = objPool.CountActive;
        countInactive = objPool.CountInactive;
        
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            // 加速生成速度，但不超过最小间隔
            spawnInterval = Mathf.Max(minSpawnInterval, spawnInterval - accelerationRate);
            
            GameObject prfb = objPool.Get();
            if (isToDestroy)
            {
                StartCoroutine(ReturnToPoolDelayed(prfb, maxExistTime));
            }
        }
    }
    

    public bool isToDestroy => maxExistTime > 0;

    private IEnumerator ReturnToPoolDelayed(GameObject obj, float delay)
    {
        Debug.Log("Waiting for " + delay + " seconds.");
        yield return new WaitForSeconds(delay);
        Debug.Log("Returning object to the pool.");
        // 将对象送回对象池
        objPool.Release(obj);
    }
}