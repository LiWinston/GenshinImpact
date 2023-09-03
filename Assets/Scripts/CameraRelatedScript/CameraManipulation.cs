using UnityEngine;
using UnityEngine.Serialization;

namespace CameraRelatedScript
{
    public class CameraManipulation : MonoBehaviour
    {
        [SerializeField]
        private Transform attachedCamera;
        
        [SerializeField]GameObject viewPoint;

        // private PlayerController playerController; // Automatically get the PlayerController component
        [SerializeField] private float forwardOffset;
        [SerializeField] private float upwardOffset;

        private void Start()
        {
            if (viewPoint == null)
            {
                Debug.LogError("眼部 not found!");
            }
            
            // playerController = GetComponent<PlayerController>();
            // if (playerController == null)
            // {
            //     Debug.LogWarning("PlayerController component not found on the same GameObject!");
            // }
        }

        private void LateUpdate()
        {
            Vector3 newPosition = viewPoint.transform.position; // Use the eye position as the base position
            
            
            // Optionally, you can add an offset forward and/or upward
            newPosition += transform.forward * forwardOffset;
            newPosition += transform.up * upwardOffset;

            attachedCamera.position = newPosition;
        }
    }
}