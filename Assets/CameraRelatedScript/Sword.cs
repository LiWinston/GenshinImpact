using UnityEngine;

public class Sword : MonoBehaviour
{
    private bool isFalling = true; // 标记光剑是否正在下落
    private Vector3 targetPosition; // 光剑目标位置

    private void Start()
    {
        targetPosition = transform.position; // 获取光剑生成位置
        transform.position = new Vector3(targetPosition.x, targetPosition.y + 20f, targetPosition.z); // 将光剑移到天上
    }

    private void Update()
    {
        if (isFalling)
        {
            // 光剑下落逻辑，使用Lerp将光剑从天上移动到目标位置
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);
            
            // 判断是否接近目标位置，如果接近则停止下落
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isFalling = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isFalling && other.CompareTag("Player"))
        {
            // 玩家被光剑击中时销毁玩家物体
            Destroy(other.gameObject);
        }

        // 光剑碰撞到任何物体都销毁光剑
        Destroy(gameObject);
    }
}