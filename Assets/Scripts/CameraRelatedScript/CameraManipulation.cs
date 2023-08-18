using UnityEngine;

public class CameraManipulation : MonoBehaviour
{
    [SerializeField]
    private Transform attachedCamera;

    [SerializeField]
    private float crouchingHeight;
    [SerializeField]Transform pirateEyesTransform;
    private float standingHeight;

    private PlayerController playerController; // Automatically get the PlayerController component

    private void Start()
    {
        if (pirateEyesTransform == null)
        {
            Debug.LogError("Pirate_Eyes not found!");
        }
        standingHeight = pirateEyesTransform.position.y;
        playerController = GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogWarning("PlayerController component not found on the same GameObject!");
        }

        crouchingHeight = playerController.crouchAmount;
    }

    private void Update()
    {
        if (playerController == null || attachedCamera == null || crouchingHeight == 0.0f)
        {
            return;
        }

        bool isCrouching = playerController.IsCrouching;

        Vector3 newPosition = transform.position; // Use attachedCamera.position instead of attachedCamera.localPosition

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