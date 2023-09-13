using UnityEngine;
using UnityEngine.Pool;

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
    // Private ObjectPool<GameObject> pool;

    private void Start()
    {
        targetPosition = transform.position;
        spawnInterval = initialSpawnInterval;
    }

    private void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            Spawn();
            spawnTimer = 0f;

            // 加速生成速度，但不超过最小间隔
            spawnInterval = Mathf.Max(minSpawnInterval, spawnInterval - accelerationRate);
        }
    }

    private void Spawn()
    {
        Vector3 spawnPosition = targetPosition + new Vector3(Random.Range(-10f, 10f), spawnHeight, Random.Range(-10f, 10f));
        GameObject newFab = Instantiate(prefab, spawnPosition, Quaternion.identity);

        if (maxExistTime > 0)
        {
            Destroy(newFab, maxExistTime);
        }
    }
}