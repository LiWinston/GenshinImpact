using Behavior;
using UnityEngine;
using Utility;

namespace ItemSystem
{
    public class TrackPlayer : MonoBehaviour
    {
        public float movementSpeed = 0.2f; // 物体的移动速度
        public float approachSpeedRate = 8f; // 接近速度，以玩家当前速度的八分之一迫近

        private Transform playerModel; // 玩家的Transform
        private Transform player; // 玩家的Transform

        private void Start()
        {
            player = PlayerController.Instance.transform;
            // 获取玩家的Transform
            playerModel = Find.FindDeepChild(PlayerController.Instance.transform, "Model");
        }

        private void Update()
        {
            // 计算物体到玩家的方向
            Vector3 directionToPlayer = playerModel.position - transform.position;

            // 计算追踪速度，以玩家当前速度的八分之一
            float trackingSpeed = player.GetComponent<Rigidbody>().velocity.magnitude / approachSpeedRate;

            // 如果物体到玩家的距离大于一定值，就移动向玩家
            if (directionToPlayer.magnitude > 0.5f)
            {
                // 移动物体向玩家
                transform.Translate(directionToPlayer.normalized * (movementSpeed + trackingSpeed) * Time.deltaTime);
            }
        }
    }
}