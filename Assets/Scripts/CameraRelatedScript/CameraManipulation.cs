using UnityEngine;
using UnityEngine.Serialization;

namespace CameraRelatedScript
{
    public class CameraManipulation : MonoBehaviour
    {
        [SerializeField]
        private Transform attachedCamera;

        [SerializeField]
        private float crouchingHeight;
        
        [FormerlySerializedAs("pirateEyesTransform")]
        [SerializeField]
        GameObject 眼部;
        private float standingHeight;

        private PlayerController playerController; // Automatically get the PlayerController component
        [SerializeField] private float forwardOffset;
        [SerializeField] private float upwardOffset;

        private void Start()
        {
            if (眼部 == null)
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
            standingHeight = 眼部.transform.position.y;
            
            if (playerController == null || attachedCamera == null || crouchingHeight == 0.0f)
            {
                return;
            }

            bool isCrouching = playerController.IsCrouching;
            Vector3 newPosition = 眼部.transform.position; // Use the eye position as the base position
            
            if (isCrouching)
            {
                newPosition.y += crouchingHeight; // Adjust the height for crouching
            }
            
            // Optionally, you can add an offset forward and/or upward
            newPosition += transform.forward * forwardOffset;
            newPosition += transform.up * upwardOffset;

            attachedCamera.position = newPosition;
        }
    }
}