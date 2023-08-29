using UnityEngine;

namespace CameraRelatedScript
{
    public class CameraManipulation : MonoBehaviour
    {
        [SerializeField]
        private Transform attachedCamera;

        [SerializeField]
        private float crouchingHeight;
        [SerializeField]GameObject pirateEyesTransform;
        private float standingHeight;

        private PlayerController playerController; // Automatically get the PlayerController component

        private void Start()
        {
            if (pirateEyesTransform == null)
            {
                Debug.LogError("Pirate_Eyes not found!");
            }
            
            
            playerController = GetComponent<PlayerController>();
            if (playerController == null)
            {
                Debug.LogWarning("PlayerController component not found on the same GameObject!");
            }

            crouchingHeight = playerController.crouchAmount;
        }

        private void LateUpdate()
        {
            standingHeight = pirateEyesTransform.transform.position.y;
            
            if (playerController == null || attachedCamera == null || crouchingHeight == 0.0f)
            {
                return;
            }

            bool isCrouching = playerController.IsCrouching;
            // Vector3 newPosition = transform.position + new Vector3(0,300,0); // Use attachedCamera.position instead of attachedCamera.localPosition
            Vector3 newPosition = transform.position;
            
            if (isCrouching)
            {
                newPosition.y = standingHeight - crouchingHeight;
            }
            else
            {
                newPosition.y = standingHeight;
            }

            attachedCamera.position = newPosition;
        }
    }
}