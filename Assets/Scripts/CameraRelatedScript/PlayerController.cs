using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float gravity = -9.81f;
    public float crouchAmount = 0.5f;
    public float crouchSpeed = 2f;

    [Header("Camera Settings")]
    public float idleShake = 0.02f;
    public float breatheShake = 0.005f;
    public float breatheSpeed = 0.2f;

    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 100f;

    private bool isMoving = false;
    private bool isJumping = false;
    private bool isCrouching = false;

    private float breatheTimer = 0f;
    private float breatheOffset = 0f;

    private Vector3 velocity;
    private CharacterController controller;
    [SerializeField]private GameObject mycamera;

    private float xRotation = 0f;

    public bool IsCrouching
    {
        get { return isCrouching; }
        set { isCrouching = value; }
    }

    private Vector3 originalScale;
    private Vector3 originalPosition;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        

        Cursor.lockState = CursorLockMode.Locked;

        originalScale = transform.localScale;
        originalPosition = transform.position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            //Check Around 
            
        }
    }

    private void LateUpdate()
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

            mycamera.transform.localPosition = new Vector3(Random.Range(-idleShake, idleShake), Random.Range(-idleShake, idleShake) + breatheOffset, 0);
        }
        else
        {
            mycamera.transform.localPosition = Vector3.zero;
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
            Vector3 newScale = new Vector3(originalScale.x, originalScale.y * (1 - crouchAmount), originalScale.z);
            Vector3 newPosition = new Vector3(originalPosition.x, originalPosition.y - (originalScale.y - newScale.y) / 2, originalPosition.z);

            transform.localScale = newScale;
            transform.position = newPosition;
        }
        else
        {
            transform.localScale = originalScale;
            transform.position = originalPosition;
        }

        // Apply movement
        controller.Move(moveDirection * (isCrouching ? crouchSpeed : moveSpeed) * Time.deltaTime);

        // Apply gravity and velocity
        controller.Move(velocity * Time.deltaTime);

        // Mouse look
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        mycamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}
