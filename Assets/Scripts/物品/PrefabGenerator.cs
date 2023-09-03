using UnityEngine;

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

    private bool isFalling = true;
    private Vector3 targetPosition;

    private void Start()
    {
        targetPosition = transform.position;
        transform.position = new Vector3(targetPosition.x, targetPosition.y + 20f, targetPosition.z);
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
        GameObject newSword = Instantiate(prefab, spawnPosition, Quaternion.identity);

        if (isFalling)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isFalling = false;
            }
        }

        if (maxExistTime > 0)
        {
            Destroy(newSword, maxExistTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isFalling && other.CompareTag("Player"))
        {
            // 玩家被击中时销毁玩家物体
            Destroy(other.gameObject);
        }

        // 碰撞到任何物体都销毁
        Destroy(gameObject);
    }
}