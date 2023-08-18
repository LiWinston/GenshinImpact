using UnityEngine;

public class SwordGenerator : MonoBehaviour
{
    public GameObject swordPrefab;
    private bool isFalling = true;
    private Vector3 targetPosition;
    public float spawnInterval = 1f;
    public float spawnHeight = 1f;

    private float spawnTimer = 0f;

    private void Start()
    {
        targetPosition = transform.position; 
        transform.position = new Vector3(targetPosition.x, targetPosition.y + 20f, targetPosition.z); 
    }
    
    private void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            SpawnSword();
            spawnTimer = 0f;
        }
    }

    private void SpawnSword()
    {
        Vector3 spawnPosition = targetPosition + new Vector3(Random.Range(-10f, 10f), spawnHeight, Random.Range(-10f, 10f));
        GameObject newSword = Instantiate(swordPrefab, spawnPosition, Quaternion.identity);
        if (isFalling)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);
            
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isFalling = false;
            }
        }
        Destroy(newSword, 10f); // 添加销毁倒计时，防止一直存在于场景中
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!isFalling && other.CompareTag("Player"))
        {
            // 玩家被击中时销毁玩家物体
            Destroy(other.gameObject);
        }

        // 碰撞到任何物体都销毁
        Destroy(gameObject);
    }
}