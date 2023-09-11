// using UnityEngine;
// using UnityEngine.Serialization;
//
 namespace CameraView{
//     public class InSightDetector : MonoBehaviour
//     {
//         [SerializeField]private float detectDistance = 1/0.0f;
//         [SerializeField]private Transform Viewer = Camera.main.transform;//By default use camera
//         
//         public bool IsInLineOfSight<T>(T target) where T : Component
//         {
//             // 获取摄像机的位置
//             Vector3 cameraPosition = Viewer.position;
//
//             // 计算从摄像机到目标物体的向量
//             Vector3 toTarget = target.transform.position - cameraPosition;
//
//             // 发射一条射线，检查是否有遮挡物
//             if (Physics.Raycast(cameraPosition, toTarget, out RaycastHit hit, detectDistance))
//             {
//                 if (hit.collider.gameObject == target.gameObject)
//                 {
//                     // 如果射线命中了目标物体，说明它在视野内
//                     return true;
//                 }
//             }
//             // 如果没有命中目标物体，说明它不在视野内
//             return false;
//         }
//         public static bool IsInLineOfSight<T>(GameObject obj,T target, float detectDistance = 1/0.0f) where T : Component
//         {
//             // 获取摄像机的位置
//             Vector3 cameraPosition = obj.transform.position;
//
//             // 计算从摄像机到目标物体的向量
//             Vector3 toTarget = target.transform.position - cameraPosition;
//
//             // 发射一条射线，检查是否有遮挡物
//             if (Physics.Raycast(cameraPosition, toTarget, out RaycastHit hit, detectDistance))
//             {
//                 if (hit.collider.gameObject == target.gameObject)
//                 {
//                     // 如果射线命中了目标物体，说明它在视野内
//                     return true;
//                 }
//             }
//             // 如果没有命中目标物体，说明它不在视野内
//             return false;
//         }
//         public static bool IsInLineOfSight<T>(Transform tr,T target, float detectDistance = 1/0.0f) where T : Component
//         {
//             // 获取摄像机的位置
//             Vector3 cameraPosition = tr.position;
//
//             // 计算从摄像机到目标物体的向量
//             Vector3 toTarget = target.transform.position - cameraPosition;
//
//             // 发射一条射线，检查是否有遮挡物
//             if (Physics.Raycast(cameraPosition, toTarget, out RaycastHit hit, detectDistance))
//             {
//                 if (hit.collider.gameObject == target.gameObject)
//                 {
//                     // 如果射线命中了目标物体，说明它在视野内
//                     return true;
//                 }
//             }
//             // 如果没有命中目标物体，说明它不在视野内
//             return false;
//         }
//
//     }
}
