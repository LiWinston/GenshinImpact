using System.Collections;
using System.Collections.Generic;
using CameraView;
using UI;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    public float crouchSpeed = 2f;
    
    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 100f;

    private bool isMoving = false;
    private bool isJumping = false;
    private bool isCrouching = false;
    public float forwardForce = 100;
    public float jumpForce = 800;
    
    
    private bool isClimbing = false;
    
    private Vector3 moveDirection;

    private float breatheTimer = 0f;
    private float breatheOffset = 0f;
    
    [SerializeField]private Camera mycamera;
    
    private Animator animator;/// <summary>
                              /// important
                              /// </summary>

    // [SerializeField] private float moveSpeedOnAttack = 0.0f; // 设置攻击时的移动速度

    private float xRotation = 0f;

    public bool IsCrouching
    {
        get { return isCrouching; }
        set { isCrouching = value; }
    }

    // private Vector3 originalScale;
    private Vector3 originalPosition;
    [SerializeField] private float MAX_ALLOWED_INTERACT_RANGE = 5;
    private bool isGrounded;
    public float moveForceTimer = 0.05f;
    public float moveForceTimerCounter = 0.05f;
    [SerializeField] private GameObject handBoneTransform;
    [SerializeField] private GameObject swordTransform;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        moveDirection = Vector3.zero;
        var childTransform = transform.Find("Model"); 
        if (childTransform != null)
        {
            animator = childTransform.GetComponent<Animator>();
        }

        if (animator == null)
        {
            Debug.LogError("找不到Animator组件！");
        }
        
        Cursor.lockState = CursorLockMode.Locked;
        originalPosition = transform.position;
    }

    private void Update()

    {
        // Vector3 handPosition = handBoneTransform.transform.position;
        //
        // // 设置剑的位置为手部骨骼的位置
        // swordTransform.transform.position = handPosition;
        
        // WASD movement
        if(!isMoving) animator.SetBool("Standing",true);
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        moveDirection = transform.right * horizontal + transform.forward * vertical;

        if (math.abs(rb.velocity.x) + math.abs(rb.velocity.z) > 0.01f)
        {
            isMoving = true;
            animator.SetBool("isMoving",isMoving);
        }
        else
        {
            isMoving = false;
            animator.SetBool("isMoving",isMoving);
        }
        
        UserInput();
        
        
        
        if (Input.GetMouseButtonDown(0)) // 检测左键点击事件
        {
            
            animator.SetTrigger("AttackTrigger");
        }
        
        // checkClimbing();
        checkInteract();
        
        if (Input.GetKeyDown(KeyCode.V)) 
        {
            animator.SetTrigger("HurricaneKickTrigger");
            rb.velocity = Vector3.zero;
        }
        
        // Crouch
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            animator.SetTrigger("BeginCrouch");
            isCrouching = true;
        }
        
        if(Input.GetKeyUp(KeyCode.LeftControl))
        {
            isCrouching = false;
        }

        
        if (Mathf.Abs(rb.velocity.y) < 0.01f)
        {
            UIManager.ShowMessage1("着地了！" + rb.velocity.y);
            isGrounded = true;
            isJumping = false; // 重置跳跃标志
        }
        else
        {
            UIManager.ShowMessage1("没着地！" + rb.velocity.y);
            isGrounded = false;
        }

        

        // Crouch
        animator.SetBool("isCrouching",isCrouching);
        
        // Mouse look
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        mycamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
    
    public void UserInput()
    {
        moveForceTimerCounter -= Time.deltaTime;
    
        if (moveForceTimerCounter <= 0)
        {
            moveForceTimerCounter += moveForceTimer;
        
            if (Input.GetKey(KeyCode.W))
            {
                rb.AddForce(transform.forward * forwardForce, ForceMode.Impulse);
            }
        
            if (Input.GetKey(KeyCode.S))
            {
                rb.AddForce(transform.forward * (-forwardForce * 0.7f), ForceMode.Impulse);
            }
        
            if (Input.GetKey(KeyCode.A))
            {
                rb.AddForce(transform.right * -forwardForce, ForceMode.Impulse);
            }
        
            if (Input.GetKey(KeyCode.D))
            {
                rb.AddForce(transform.right * forwardForce, ForceMode.Impulse);
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isJumping) // 添加对是否已经跳跃的检查
        {
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("Jump");
            isJumping = true; // 标记已经跳跃
            isGrounded = false;
        }
    }

    
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
}
