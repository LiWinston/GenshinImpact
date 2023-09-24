using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

[System.Serializable]
public class RemoteSpelling: MonoBehaviour
{
    [InspectorLabel("生成--GenerationInfo")]
    public GameObject prefab;
    [SerializeField] private float maxExistTime = 10f;
    
    private Vector3 targetPosition;

    [InspectorLabel("对象池--ObjectPool")]
    [SerializeField]private int defaultCapacity = 10;
    [SerializeField]private int maxCapacity = 10;
    private ObjectPool<GameObject> throwingsPool;
    public int countAll;
    public int countActive;
    public int countInactive;
    
    private void Start()
    {
        targetPosition = transform.position;
        throwingsPool = new ObjectPool<GameObject>(CreateFunc, actionOnGet, actionOnRelease, actionOnDestroy,
            true, defaultCapacity, maxCapacity);
    }
    GameObject CreateFunc()
    {
        
        var throwing = Instantiate(prefab, targetPosition, Quaternion.identity);
        throwing.GetComponent<IPoolable>().SetPool(throwingsPool);
        return throwing;
    }
    
    void actionOnGet(GameObject obj)
    {
        obj.transform.position = targetPosition;
        obj.GetComponent<IPoolable>().actionOnGet();
        obj.SetActive(true);
    }

    void actionOnRelease(GameObject obj)
    {
        obj.GetComponent<IPoolable>().actionOnRelease();
        obj.SetActive(false);
    }

    void actionOnDestroy(GameObject obj)
    {
        Destroy(obj);
    }
    
    private void Update()
    {
        countAll = throwingsPool.CountAll;
        countActive = throwingsPool.CountActive;
        countInactive = throwingsPool.CountInactive;
        
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            // Accelerate the generation speed, but do not exceed the minimum interval
            spawnInterval = Mathf.Max(minSpawnInterval, spawnInterval - accelerationRate);
            
            GameObject prfb = throwingsPool.Get();
            if (isToDestroy)
            {
                StartCoroutine(ReturnToPoolDelayed(prfb, maxExistTime));
            }
        }
    }
    

    public bool isToDestroy => maxExistTime > 0;

    private IEnumerator ReturnToPoolDelayed(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        // Return the object to the object pool
        if (obj.activeSelf)
        {
            throwingsPool.Release(obj);
        }
    }
}