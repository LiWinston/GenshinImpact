using UnityEngine;
using UnityEngine.Serialization;

namespace CameraRelatedScript
{
    public class CameraManipulation : MonoBehaviour
    {
        [SerializeField]
        private Transform attachedCamera;
        
        [SerializeField] GameObject viewPoint;
        [SerializeField] GameObject FPCamera; // 备用相机
        [SerializeField] private LayerMask wallLayer; // 墙体的Layer

        [SerializeField] private float forwardOffset;
        [SerializeField] private float upwardOffset;
        [SerializeField] private float FPforwardOffset;
        [SerializeField] private float FPupwardOffset;
        private float transitionSpeed = 1.0f; // 调整过渡速度
        private bool obstacleInWay = false; // 添加一个标志来表示是否有障碍物

        private void Start()
        {
            if (viewPoint == null)
            {
                Debug.LogError("viewPoint not found!");
            }
            
            if (FPCamera == null)
            {
                Debug.LogError("FPCamera not found!");
            }
        }

        private void LateUpdate()
        {
            Vector3 newPosition = viewPoint.transform.position;

            // 使用射线检测来避免相机被墙体阻挡
            RaycastHit hit;
            Vector3 cameraToViewPoint = viewPoint.transform.position - attachedCamera.position;

            if (Physics.Raycast(attachedCamera.position, cameraToViewPoint, out hit, cameraToViewPoint.magnitude, wallLayer))
            {
                if (!obstacleInWay)
                {
                    // 切换到备用相机
                    FPCamera.SetActive(true);
                    attachedCamera.gameObject.SetActive(false);
                    obstacleInWay = true;
                }

                // 使用备用相机的位置
                Vector3 newFPPosition = newPosition + transform.forward * FPforwardOffset + transform.up * FPupwardOffset;
                FPCamera.transform.position = Vector3.Lerp(FPCamera.transform.position, newFPPosition, Time.deltaTime * transitionSpeed);
            }
            else
            {
                if (obstacleInWay)
                {
                    // 切换回原相机
                    FPCamera.SetActive(false);
                    attachedCamera.gameObject.SetActive(true);
                    obstacleInWay = false;
                }

                newPosition += transform.forward * forwardOffset + transform.up * upwardOffset;
                // 平滑过渡到新位置
                attachedCamera.position = Vector3.Lerp(attachedCamera.position, newPosition, Time.deltaTime * transitionSpeed);
            }
        }
    }
}
