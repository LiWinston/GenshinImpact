using System;
using System.Collections;
using System.Collections.Generic;
using AttributeRelatedScript;
using CameraView;
using CodeMonkey.HealthSystemCM;
using TMPro;
using UI;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField]Transform viewPoint;
    
    [Header("Movement Settings")]
    public float crouchForceRate = 0.95f;
    [SerializeField] private float MaxCrouchPlySpeed = 1f;
    [SerializeField] private float MaxPlySpeed = 2f;
    
    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 100f;

    private bool isMoving = false;
    private bool isJumping = false;
    private bool isCrouching = false;
    public float forwardForce = 100;
    public float backwardRate = 0.9f;
    public float jumpForce = 800;
    
    
    // private bool isClimbing = false;
    
    private Vector3 moveDirection;

    
    private float lastAttackTime = 0f; 
    
    [SerializeField]private Camera mycamera;
    
    private Animator animator;
    

    private float xRotation = 0f;

    private bool isNextAttackCritical = false;
    public delegate void OnAttackEndedHandler();
    public event OnAttackEndedHandler OnAttackEnded;

    public bool IsCrouching
    {
        get { return isCrouching; }
        set { isCrouching = value; }
    }

    private Vector3 originalPosition;
    [SerializeField] private float MAX_ALLOWED_INTERACT_RANGE = 3;
    private bool isGrounded;
    public float moveForceTimer = 0.05f;
    public float moveForceTimerCounter = 0.05f;
    
    [SerializeField] private GameObject swordTransform;
    private float speed_Ratio_Attack = 0.1f;
    public float rotationFriction = 4000f; // 调整旋转摩擦力的大小
    private State state;
    [SerializeField] private Transform sword;
    internal bool cheatMode = false;
    private CriticalHitCurve _criticalHitCurve;

    private void Start()
    {
        _criticalHitCurve = GetComponent<CriticalHitCurve>();
        state = GetComponent<State>();
        rb = GetComponent<Rigidbody>();
        moveDirection = Vector3.zero;
        var model = transform.Find("Model"); 
        if (model != null)
        {
            animator = model.GetComponent<Animator>();
        }
        else
        {
            Debug.LogError("Didnt Find 'model'!");
        }

        viewPoint = SpellCast.FindDeepChild(transform, "root");//调一下得了 不粘贴过来了
        sword = SpellCast.FindDeepChild(transform, "blade");
        if (viewPoint == null)
        {
            Debug.LogError("viewPoint not found!");
        }
        if (animator == null)
        {
            Debug.LogError("Didnt Find animator!");
        }
        
        Cursor.lockState = CursorLockMode.Locked;
        originalPosition = transform.position;
        if (Camera.main != null) Camera.main.transform.rotation = Quaternion.identity; // 正前方
        animator.SetFloat("AttSpeedMult",1f);
    }

    private void Update()

    {
        if (Mathf.Abs(rb.velocity.y) < 0.1f)
        {
            isGrounded = true;
            isJumping = false; // reset
        }
        else
        {
            isGrounded = false;
        }
        animator.SetBool("isGrounded",isGrounded);
        
        isMoving = math.abs(rb.velocity.x) + math.abs(rb.velocity.z) > 0.01f;

        animator.SetBool("Standing",!isMoving);
        animator.SetBool("isMoving",isMoving);

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        moveDirection = transform.right * horizontal + transform.forward * vertical;
        
        UserInput();
        

        if (Input.GetKeyDown(KeyCode.E))
        {
            // StartCoroutine("checkInteract");
            checkInteract();
        }
        
        if (Input.GetKeyDown(KeyCode.V)) 
        {
            animator.SetTrigger("HurricaneKickTrigger");
        
            HurricaneKick();
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

    private void HurricaneKick()
    {
        rb.velocity = Vector3.zero;
        Vector3 playerPosition = transform.position;
        Collider[] hitEnemies = Physics.OverlapSphere(playerPosition, state.hurricaneKickRange);

        // UIManager.Instance.ShowMessage2(hitEnemies.Length == 0 ? "What are you kicking?" : "Lets KICK!");

        foreach (Collider cld in hitEnemies)
        {
            if (cld.CompareTag("Enemy"))
            {
                Vector3 enemyPosition = cld.transform.position;
                
                Vector3 knockbackDirection = (enemyPosition - playerPosition).normalized;
                
                HealthSystem enemyHealth = cld.GetComponent<HealthSystemComponent>().GetHealthSystem();

                cld.GetComponentInChildren<Animator>().SetTrigger("HurricaneKickTrigger");
                if (enemyHealth != null)
                {
                    enemyHealth.Damage(state.HurricaneKickDamage);
                    // UI.UIManager.Instance.ShowMessage2("What a Hurricane Kick!");
                    Rigidbody enemyRigidbody = cld.GetComponent<Rigidbody>();
                    if (enemyRigidbody != null)
                    {
                        enemyRigidbody.AddForce(knockbackDirection * state.hurricaneKickKnockbackForce, ForceMode.VelocityChange);
                        // 计算随机切向方向（左或右）
                        Vector3 randomTangentDirection = Quaternion.Euler(0, Random.Range(-90f, 90f), 0) * knockbackDirection;

                        // 计算旋转摩擦力，不依赖于当前角速度
                        Vector3 rotationFrictionForce = randomTangentDirection * rotationFriction;

                        // 将旋转摩擦力施加到切向方向
                        enemyRigidbody.AddTorque(rotationFrictionForce * Random.Range(0.5f, 1.5f), ForceMode.Impulse);                    }
                }
            }
        }
    } 

 
    
    /// <summary>
    /// /abandoned Attack function using range detect and angle limiting
    /// </summary>
    /*
    private void Attack_Backup()
    {
        rb.velocity *= speed_Ratio_Attack;
        UI.UIManager.Instance.ShowMessage2("Taste My Sword !!!(While a little stupid)");
        animator.SetTrigger("AttackTrigger");
        // var sword = SpellCast.FindDeepChild(transform, "Scabbard");
        // ParticleEffectManager.Instance.PlayParticleEffect("Attack", sword.gameObject, Quaternion.identity,Color.white, Color.white);
        Vector3 characterPosition = transform.position;
        
        Vector3 targetDirection = transform.forward;
        
        var attackAngle = damage.attackAngle;
        var attackRange = damage.attackRange;
        float attackRadius = Mathf.Tan(Mathf.Deg2Rad * (attackAngle / 2f)) * attackRange;
        
        Collider[] enemies = Physics.OverlapSphere(characterPosition, attackRange);
        foreach (Collider enemyCollider in enemies)
        {
            // Check if enemy
            if (enemyCollider.CompareTag("Enemy"))
            {
                // Calculate the vector of the main character to the enemy
                Vector3 enemyPosition = enemyCollider.transform.position;
                Vector3 direction = enemyPosition - characterPosition;

                // Calculate the angle between the protagonist and the enemy
                float angle = Vector3.Angle(targetDirection, direction);

                // Check if the angle is within the attack angle range
                if (angle <= attackAngle / 2f)
                {
                    // Check for HealthSystem components
                    HealthSystem healthSystem = enemyCollider.GetComponent<HealthSystemComponent>().GetHealthSystem();
                    if (healthSystem != null)
                    {
                        UIManager.Instance.ShowMessage1("A "+damage.CurrentDamage+" Cut~");
                        healthSystem.Damage(damage.CurrentDamage); // Inflict damage on enemies
                    }
                }
            }
        }
    }
    */




    public void UserInput()
    {
        if (Input.GetMouseButtonDown(0) && Time.time - lastAttackTime >= state.AttackCooldown)
        {
            rb.velocity *= speed_Ratio_Attack;
            
            float criticalHitChance = _criticalHitCurve.CalculateCriticalHitChance(state.GetCurrentLevel());
            // Debug.Log(state.GetCurrentLevel() + "级暴击率" + criticalHitChance*100 +"%");

            var randomValue = Random.Range(0.0f, 1.0f);
            isNextAttackCritical = randomValue <= criticalHitChance;
            StartCoroutine(isNextAttackCritical ? CriticalAttack() : NormalAttack());
            lastAttackTime = Time.time;
        }
        
        if (Input.GetKeyDown(KeyCode.Space) ) // Add a check to see if a jump has been made
        {
            if(isGrounded){
                if(isJumping) return;
                if(!isMoving){
                    rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
                    animator.SetTrigger("Jump");
                    isJumping = true;
                    isGrounded = false;
                }else if(isMoving){
                    rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
                    animator.SetTrigger("RunningJump");
                    isJumping = true;
                    isGrounded = false;
                }
            }
            
        }

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            if (cheatMode == false)
            {
                showExp("CHEAT MODE ON");
                cheatMode = true;
            }
            else
            {
                showExp("CHEAT MODE OFF");
                cheatMode = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            if (cheatMode)
            {
                state.CheatLevelUp();
            }
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.K))
        {
            if (cheatMode)
            {
                // 如果作弊模式开启，对玩家造成最大生命值的伤害
                state.TakeDamage(999999999f);
            }
        }
        
        
        moveForceTimerCounter -= Time.deltaTime;

        if (!(moveForceTimerCounter <= 0))
        {
            return;
        }

        moveForceTimerCounter += moveForceTimer;
        var f = isCrouching ? crouchForceRate * forwardForce : forwardForce;
        
        Vector3 moveDirection = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            moveDirection += transform.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDirection -= transform.forward * backwardRate; // 向后移动的力减小
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDirection -= transform.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDirection += transform.right;
        }
        
        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }
        
        rb.AddForce(moveDirection * f, ForceMode.Force);
        
        float maxSpeed = isCrouching ? MaxCrouchPlySpeed : MaxPlySpeed;
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);

    }

    private IEnumerator NormalAttack()
    {
        animator.SetTrigger("AttackTrigger1");
        animator.SetBool("isAttacking", true);
        // yield return null; // Wait for one frame to let animation begin
        // yield return null; // Wait for another frame to ensure animation state is updated

        // AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        // float animationLength = stateInfo.length;
        // yield return new WaitForSeconds(animationLength);
        yield return new WaitForSeconds(0.75f * 1.25f / state.attackSpeedRate); //这是一个土狗解决方案
    
        // animator.SetBool("isAttacking", false);
        EndAttack();
    }

    private IEnumerator CriticalAttack()
    {
        animator.SetTrigger("AttackTrigger2");
        animator.SetBool("isAttacking", true);
        // yield return null; // Wait for one frame to let animation begin
        // yield return null; // Wait for another frame to ensure animation state is updated
        //
        // AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        // float animationLength = stateInfo.length;
        yield return new WaitForSeconds(0.875f / (0.75f * state.attackSpeedRate)); //这是一个土狗解决方案
        
        EndAttack();
    }



    private void checkInteract()
    {
        // InSightDetector sightDetector = new InSightDetector();
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
                // if ((distance < pickable.pickupRange) && sightDetector.IsInLineOfSight(viewPoint,pickable))  //the ray tracer detect always obstacle the picking, 先关了
                if ((distance < pickable.pickupRange))
                {
                    // transform.LookAt(pickable.transform);
                    
                    //TODO:动画播完在触发Pick()
                    rb.velocity = Vector3.zero;
                    StartCoroutine(GoPick(pickable));
                    
                    hasPickable = true;
                    break;
                }
            }
        }

        if (!hasPickable)
        {
            UIManager.Instance.ShowMessage1("Noooooooooooooooooo way!");
        }
    }


    private IEnumerator GoPick(Pickable pickable)
    {
        // 获取目标方向
        Vector3 targetDirection = pickable.transform.position - transform.position;
        targetDirection.y = 0f; // 将Y轴分量置零，以确保只在水平面上旋转

        // 计算旋转角度
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        // 设置最大旋转角度
        float maxRotationAngle = 360f; // 调整最大旋转角度

        // 触发"Picking"动画
        animator.SetTrigger("Picking");

        // 获取动画的长度
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length;

        // 旋转到目标方向
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            // 计算旋转步长
            float step = maxRotationAngle * Time.deltaTime;

            // 使用RotateTowards旋转
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);
        
            yield return null;
        }

        // 等待动画播放完毕
        yield return new WaitForSeconds(animationLength);

        // 执行捡取操作
        pickable.Pick();
    }


    
    private Collider[] TryToInteract()
    {
        return Physics.OverlapSphere(transform.position, MAX_ALLOWED_INTERACT_RANGE);
    }

    
    public void showExp(string expText)
    {
        // 查找场景内所有名称为 "ExpText" 的对象
        TextMeshPro[] expTextObjects = Resources.FindObjectsOfTypeAll<TextMeshPro>();
        // TextMeshPro[] expTextObjects = GetComponentsInChildren<TextMeshPro>();
        // if(expTextObjects.Length == 0){ShowMessage1("No txterPro");}

        foreach (TextMeshPro textMesh in expTextObjects)
        {
            if (textMesh != null)
            {
                textMesh.text = expText;
                textMesh.alpha = 1f; // 设置初始透明度为1，完全可见
                textMesh.gameObject.SetActive(true);

                // 启动协程来淡出经验值显示
                // MonoBehaviour monoBehaviour = textMesh.gameObject.GetComponent<MonoBehaviour>();
                // monoBehaviour.StartCoroutine(FadeOutExpText(textMesh));
                StartCoroutine(FadeOutExpText(textMesh));
            }
            else
            {
                Debug.LogError("TextMeshPro component not found on an ExpText GameObject.");
            }
        }
    }

    private static IEnumerator FadeOutExpText(TextMeshPro textMesh, float time = 0.8f, float fadeTime = 0.5f)
    {
        // 延迟一段时间以便观察经验值文本
        yield return new WaitForSeconds(time);

        float fadeDuration = fadeTime;
        float startAlpha = textMesh.alpha;
        float currentTime = 0f;

        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, 0f, currentTime / fadeDuration);
            textMesh.alpha = newAlpha;
            yield return null;
        }

        textMesh.gameObject.SetActive(false);
    }

    public void TakeDamage(float dmg)
    {
        animator.SetTrigger("Hurt");
        state.TakeDamage(dmg);
        if (state.IsEmptyHealth())
        {
            StartCoroutine(GameOver());
        }
    }

    private IEnumerator GameOver()
    {
        animator.Play("Flying Back Death");
        yield return new WaitForSeconds(3.1f);
        SceneManager.LoadScene("LoseScene"); 
    }

    public float GetDamage()
    {
        return (isNextAttackCritical ? state.criticalDmgRate * state.CurrentDamage : state.CurrentDamage);
    }

    public void EndAttack()
    {
        // 触发结束攻击事件
        
        OnAttackEnded();
    }
    
    public Animator GetAnimator()
    {
        return animator;
    }

    public void UpdateAttackAnimationTime(float attackSpeedRate)
    {
        animator.SetFloat("AttSpeedMult",attackSpeedRate);
    }
}
