using System.Collections;
using System.Collections.Generic;
using CameraView;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    // public float jumpForce = 7f;
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
    
    public float climbingSpeed = 2.0f; // 爬墙速度
    public float maxClimbingHeight = 5.0f; // 最大爬升高度
    private bool isClimbing = false;
    private float initialHeight;
    private Vector3 moveDirection = Vector3.zero;

    private float breatheTimer = 0f;
    private float breatheOffset = 0f;

    private Vector3 velocity = Vector3.zero;
    private CharacterController controller;
    [SerializeField]private GameObject mycamera;
    
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

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        // 查找子物体中的Animator组件
        var childTransform = transform.Find("Model"); // 替换为子物体的名称
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
        if(!isMoving) animator.SetBool("Standing",true);
        // WASD movement
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;

        if (velocity.magnitude >= 0.001f)
        {
            isMoving = true;
            // if (velocity.magnitude >= 0.0f) isRunning = true;
        }
        else
        {
            isMoving = false;
        }
        
        if(isMoving == true) animator.SetBool("isMoving",true);
        if (Input.GetMouseButtonDown(0)) // 检测左键点击事件
        {
            // StartCoroutine(DecreaseSpeedAndDisableRunning());// 开始协程执行骤减速度和设置isRunning的操作
            animator.SetTrigger("AttackTrigger");// 触发攻击动作
        }
        initialHeight = transform.position.y;
        checkClimbing();
        checkInteract();
        
        if (Input.GetKeyDown(KeyCode.V)) // 以按下H键触发为例
        {
            // 设置触发器参数为true，触发动画
            animator.SetTrigger("HurricaneKickTrigger");
        }
        
        // Crouch
        if(Input.GetKeyDown(KeyCode.LeftControl)) animator.SetTrigger("BeginCrouch");
        if (Input.GetKey(KeyCode.LeftControl))
        {
            isCrouching = true;
        }
        else
        {
            isCrouching = false;
        }


        // Apply gravity
        if (!controller.isGrounded)
        {
            UIManager.ShowMessage1("没着地！");
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
            animator.SetBool("isCrouching",true);
        }
        else
        {
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
    // private IEnumerator DecreaseSpeedAndDisableRunning()
    // {
    //     if (isMoving)
    //     {
    //         // 设置较大的加速度以骤减速度
    //         float initialSpeed = velocity.magnitude;
    //         float targetSpeed = moveSpeedOnAttack;
    //         float acceleration = (targetSpeed - initialSpeed) / Time.deltaTime;
    //
    //         while (velocity.magnitude > targetSpeed)
    //         {
    //             // 计算减速的方向
    //             Vector3 decelerationDirection = -velocity.normalized;
    //             // 计算应用的减速度
    //             Vector3 deceleration = decelerationDirection * (acceleration * Time.deltaTime);
    //
    //             // 更新速度
    //             velocity += deceleration;
    //
    //             // 等待一帧
    //             yield return null;
    //         }
    //
    //         // 骤减速度完成后，将isRunning置为false
    //         isMoving = false;
    //     }
    // }


    private void checkClimbing()
    {
        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded && !isClimbing)
        {
            isJumping = true;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 1.0f) && hit.collider.CompareTag("Wall"))
            {
                // 如果墙体检测到，进入爬墙状态
                isClimbing = true;
                moveDirection = Vector3.up * climbingSpeed;
            }
        }
        if (controller.isGrounded)
        {
            // 如果在地面上，重置爬墙状态和速度
            isClimbing = false;
            moveDirection = Vector3.zero;
            
        }
        

        if (isClimbing)
        {
            UIManager.ShowMessage1("开爬！");
            // 如果在爬墙状态
            moveDirection = new Vector3(0, climbingSpeed, 0);
            controller.Move(moveDirection * Time.deltaTime);

            // 检查是否达到最大爬升高度
            if (transform.position.y - initialHeight >= maxClimbingHeight)
            {
                // 超过最大高度，退出爬墙状态，模拟下落
                isClimbing = false;
                // StartCoroutine(FallFromWall());
            }
        }

        // 应用重力
        if (!isClimbing)
        {
            moveDirection.y -= gravity * Time.deltaTime;
            controller.Move(moveDirection * Time.deltaTime);
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
    }
}
