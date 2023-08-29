using System;
using System.Collections.Generic;
using System.ComponentModel;
using CameraView;
using UI;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
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
    [SerializeField] private float MAX_ALLOWED_INTERACT_RANGE = 5;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        

        Cursor.lockState = CursorLockMode.Locked;

        originalScale = transform.localScale;
        originalPosition = transform.position;
    }

    private void Update()
    {
        checkInteract();
    }

    // private void checkInteract()
    // {
    //     if (!Input.GetKeyDown(KeyCode.E)) return;
    //     var hasPickable = false;
    //     //Check Around 
    //     var nearColliders = TryToInteract();
    //     if (nearColliders is { Length: > 0 })
    //     {
    //         Pickable nearestPickable = null;
    //         var distance = 1 / 0.0f;
    //         foreach (var cld in nearColliders)
    //         {
    //             var pkb = cld.GetComponent<Pickable>();
    //             if (null == pkb) continue;
    //             if (distance <= pkb.pickupRange &&
    //                 Vector3.Distance(transform.position, pkb.transform.position) < 
    //                     math.min(distance,MAX_ALLOWED_INTERACT_RANGE) )
    //             {
    //                 distance = Vector3.Distance(transform.position, pkb.transform.position);
    //                 nearestPickable = pkb;
    //                 hasPickable = true;
    //                 
    //             }
    //         }
    //         if (nearestPickable != null)
    //         {
    //             nearestPickable.Pick();
    //         }
    //     };
    //     if (hasPickable == false)
    //     {
    //         UIManager.ShowMessage1("Noooooooooooooooooo way!");
    //     }
    // }
    private void checkInteract()
    {
        InSightDetect sightDetector = new InSightDetect();
        if (!Input.GetKeyDown(KeyCode.E)) return;
        var hasPickable = false;
        // Check Around
        var nearColliders = TryToInteract();
        if (nearColliders is { Length: > 0 })
        {
            // Create a list to store pickable objects and their distances.
            List<(Pickable pickable, float distance)> pickableList = new List<(Pickable, float)>();

            foreach (var cld in nearColliders)
            {
                var pkb = cld.GetComponent<Pickable>();
                if (null == pkb) continue;

                float currentDistance = Vector3.Distance(transform.position, pkb.transform.position);

                // Only consider pickable objects within MAX_ALLOWED_INTERACT_RANGE.
                if (currentDistance <= MAX_ALLOWED_INTERACT_RANGE)
                {
                    pickableList.Add((pkb, currentDistance));
                }
            }

            // Sort the list by distance, from nearest to farthest.
            pickableList.Sort((a, b) => a.distance.CompareTo(b.distance));

            foreach (var (pickable, distance) in pickableList)
            {
                if ((distance < pickable.pickupRange) && sightDetector.IsInLineOfSight(pickable))
                {
                    pickable.Pick();
                    hasPickable = true;
                    break;
                }
            }
        }

        if (!hasPickable)
        {
            UIManager.ShowMessage1("Noooooooooooooooooo way!");
        }
    }


    private Collider[] TryToInteract()
    {
        return Physics.OverlapSphere(transform.position, MAX_ALLOWED_INTERACT_RANGE);
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
