using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using IPoolable = Utility.IPoolable;
using Random = UnityEngine.Random;

namespace ItemSystem.Generate
{
    [System.Serializable]
    public class PrefabGenerator : MonoBehaviour
    {
        [InspectorLabel("生成--GenerationInfo")]
        public GameObject prefab;
        [SerializeField] private float initialSpawnInterval = 1f;
        [SerializeField] private float minSpawnInterval = 0.1f;
        [SerializeField] private float spawnHeight = 1f;
        [SerializeField] private float accelerationRate = 0.1f;
        [SerializeField] private float maxExistTime = 10f;
    
        private float spawnTimer = 0f;
        private float spawnInterval;
        private Vector3 targetPosition;

        [InspectorLabel("对象池--ObjectPool")]
        [SerializeField]private int defaultCapacity = 40;
        [SerializeField]private int maxCapacity = 100;
        private ObjectPool<GameObject> objPool;
        public int countAll;
        public int countActive;
        public int countInactive;
    
        private void Start()
        {
            targetPosition = transform.position;
            spawnInterval = initialSpawnInterval;
            objPool = new ObjectPool<GameObject>(CreateFunc, actionOnGet, actionOnRelease, actionOnDestroy,
                true, defaultCapacity, maxCapacity);
        }
        GameObject CreateFunc()
        {
            Vector3 spawnPosition = targetPosition + new Vector3(Random.Range(-10f, 10f), spawnHeight, Random.Range(-10f, 10f));
            var prfb = Instantiate(prefab, spawnPosition, Quaternion.identity);
            prfb.GetComponent<IPoolable>().SetPool(objPool);
            // SetPoolForGeneratedObject(prfb);
            prfb.name = countAll.ToString();
            return prfb;
        }
    
        void actionOnGet(GameObject obj)
        {
            obj.GetComponent<IPoolable>().actionOnGet();
            obj.transform.position = targetPosition + new Vector3(Random.Range(-10f, 10f), spawnHeight, Random.Range(-10f, 10f));
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
            countAll = objPool.CountAll;
            countActive = objPool.CountActive;
            countInactive = objPool.CountInactive;
        
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnInterval)
            {
                spawnTimer = 0f;
                // Accelerate the generation speed, but do not exceed the minimum interval
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
            if (!obj.GetComponent<IPoolable>().IsExisting) yield break;
            yield return new WaitForSeconds(delay);
            // Return the object to the object pool
            if (obj.activeSelf)
            {
                objPool.Release(obj);
            }
        }
    }
}