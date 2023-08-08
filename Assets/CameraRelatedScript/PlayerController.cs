using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float gravity = -9.81f;
    public float crouchAmount = 0.5f;
    public float crouchSpeed = 5f;

    [Header("Camera Settings")]
    public float idleShake = 0.02f;
    public float breatheShake = 0.005f;
    public float breatheSpeed = 0.2f;

    private bool isMoving = false;
    private bool isJumping = false;
    private bool isCrouching = false;

    private float breatheTimer = 0f;
    private float breatheOffset = 0f;

    private Vector3 velocity;
    private CharacterController controller;
    private Transform cameraTransform;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        // Breathing shake
        if (!isMoving)
        {
            breatheTimer += Time.deltaTime;
            if (breatheTimer > breatheSpeed)
            {
                breatheOffset = Mathf.Lerp(-breatheShake, breatheShake, Mathf.PingPong(breatheTimer, 2 * breatheSpeed) / (2 * breatheSpeed));
                breatheTimer -= breatheSpeed;
            }

            cameraTransform.localPosition = new Vector3(Random.Range(-idleShake, idleShake), Random.Range(-idleShake, idleShake) + breatheOffset, 0);
        }
        else
        {
            cameraTransform.localPosition = Vector3.zero;
        }

        // WASD movement
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;

        if (moveDirection != Vector3.zero)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
        {
            isJumping = true;
        }

        // Crouch
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            isCrouching = false;
        }

        // Apply gravity
        if (!controller.isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else if (velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Jump logic
        if (isJumping)
        {
            float jumpHeight = GetComponent<Jumpability>() ? GetComponent<Jumpability>().JumpHeight : 0.7f;
            velocity.y = Mathf.Sqrt(jumpHeight * 2f * Mathf.Abs(gravity));
            isJumping = false;
        }

        // Crouch logic
        if (isCrouching)
        {
            transform.localScale = new Vector3(1, 1 - crouchAmount, 1);
        }
        else
        {
            transform.localScale = Vector3.one;
        }

        // Apply movement
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        // Apply gravity and velocity
        controller.Move(velocity * Time.deltaTime);
    }
}
