using UnityEngine;

public class SwordGenerator : MonoBehaviour
{
    public GameObject swordPrefab;
    public float spawnInterval = 3f;
    public float spawnHeight = 10f;

    private float spawnTimer = 0f;

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
        Vector3 spawnPosition = new Vector3(Random.Range(-10f, 10f), spawnHeight, Random.Range(-10f, 10f));
        GameObject newSword = Instantiate(swordPrefab, spawnPosition, Quaternion.identity);
        Destroy(newSword, 10f); // 添加光剑销毁倒计时，防止一直存在于场景中
    }
}