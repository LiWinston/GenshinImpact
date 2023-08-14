using UnityEngine;

public class CameraManipulation : MonoBehaviour
{
    [SerializeField]
    private Transform attachedCamera;

    [SerializeField]
    private float crouchingHeight;

    [SerializeField]
    private float standingHeight = 1.0f;

    private PlayerController playerController; // Automatically get the PlayerController component

    private void Start()
    {
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

        Vector3 newPosition = attachedCamera.localPosition;

        if (isCrouching)
        {
            newPosition.y -= crouchingHeight;
        }
        else
        {
            newPosition.y = standingHeight;
        }

        attachedCamera.localPosition = newPosition;
    }
}
