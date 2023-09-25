using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using enemyBehaviour;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using Utility;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

[System.Serializable]
public class RemoteSpelling: MonoBehaviour
{
    [InspectorLabel("生成--GenerationInfo")]
    public GameObject prefab;
    [SerializeField] private float maxExistTime = 10f;
    internal LayerMask castingLayer;
    [SerializeField] protected Vector3 generatingOffset = Vector3.up * 0.4f;
    

    [InspectorLabel("对象池--ObjectPool")]
    [SerializeField]private int defaultCapacity = 10;
    [SerializeField]private int maxCapacity = 10;
    protected ObjectPool<GameObject> _throwingsPool;
    public int countAll;
    public int countActive;
    public int countInactive;
    
    [InspectorLabel("Trying Cast Position")]
    private GameObject SkillPreview;
    private LineRenderer lineRenderer;
    private bool isCasting;
    private Coroutine castingCoroutine;
    public float castingDistance = 7f; // 施法最大距离
    private Vector3 hitTarget;
    protected RemoteThrowingsBehavior throwingsBehavior;
    protected PlayerController _playerController;
    private SpellCast _spellCast;
    private bool canCast = false;

    [InspectorLabel("Skill Customization")]
    [SerializeField]
    protected string animatorTriggerName;
    [SerializeField] protected float animationGap = 0.4f;
    [SerializeField] private KeyCode key = KeyCode.F;
    [SerializeField] private Color validColor = Color.green;
    [SerializeField] private Color invalidColor = Color.red;

    [InspectorLabel("Throwing Customization")]
    [SerializeField] public bool isUpdatedWithLevel = false;
    
    private void Start(){
        string randomName;
        do
        {
            randomName = UnityEngine.Random.Range(0, 100000).ToString();
        } while (GameObject.Find(randomName) != null);
        SkillPreview = new GameObject(randomName);
        SkillPreview.transform.SetParent(this.transform);
        _spellCast = GetComponent<SpellCast>();
        _playerController = GetComponent<PlayerController>();
        _throwingsPool = new ObjectPool<GameObject>(CreateFunc, actionOnGet, actionOnRelease, actionOnDestroy,
            true, defaultCapacity, maxCapacity);
        castingLayer = LayerMask.GetMask("Wall", "Floor");
        throwingsBehavior = prefab.GetComponent<RemoteThrowingsBehavior>();

        // 初始化 LineRenderer
        lineRenderer = SkillPreview.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
    }

    private GameObject CreateFunc(){
        // if(countAll < maxCapacity){
        //     GameObject throwing = Instantiate(prefab, transform.position, Quaternion.identity);
        //     actionOnGet(throwing);   //Try a new way of coding to solve the problem of non-destruction
        //     throwing.GetComponent<IPoolable>().SetPool(_throwingsPool);
        //     // throwing.GetComponent<RemoteThrowingsBehavior>().actionOnGet();
        //     return throwing.GameObject();
        // }
        // else
        // {
        //     _throwingsPool.Dispose();
        //     return _throwingsPool.Get();
        // }
        GameObject throwing = Instantiate(prefab, transform.position, Quaternion.identity);
        actionOnGet(throwing);
        throwing.GetComponent<IPoolable>().SetPool(_throwingsPool);
        return throwing.GameObject();
    }

    private void actionOnGet(GameObject obj){
        obj.GetComponent<IPoolable>().actionOnGet();
        obj.SetActive(true);
    }

    private void actionOnRelease(GameObject obj)
    {
        obj.GetComponent<IPoolable>().actionOnRelease();
        obj.SetActive(false);
    }

    void actionOnDestroy(GameObject obj)
    {
        // Destroy(obj);
    }
    
    private void Update()
    {
        countAll = _throwingsPool.CountAll;
        countActive = _throwingsPool.CountActive;
        countInactive = _throwingsPool.CountInactive;
        if (throwingsBehavior.positionalCategory == RemoteThrowingsBehavior.PositionalCategory.Throwing)
        {
            if (Input.GetKeyDown(key))
            {
                if(isUpdatedWithLevel) StartCoroutine(StartThrowWithUpdate());
                else
                {
                    StartCoroutine(StartThrow());
                }
            }
        }
        if (throwingsBehavior.positionalCategory == RemoteThrowingsBehavior.PositionalCategory.ImmediatelyInPosition)
        {
            if (Input.GetKeyDown(key))
            {
                StartCasting();
            }
            else if (Input.GetKeyUp(key))
            {
                if (castingCoroutine != null)
                {
                    StopCoroutine(castingCoroutine);
                }
                StartCoroutine(StopCasting());
            }
        }
    }

    private IEnumerator StartThrowWithUpdate(){
        _playerController.GetAnimator().SetTrigger(animatorTriggerName = "Throw");
        _playerController.isCrouching = false;
        _playerController.rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(animationGap);

        int maxSwordCount = 30; // 最大剑气数量
        int minSwordCount = 1; // 最小剑气数量
        float maxAngle = 30f; // 最大角度
        float minAngle = 0f; // 最小角度

        int playerLevel = _playerController.state.GetCurrentLevel(); // 获取玩家等级
        int swordCount = Mathf.Clamp(playerLevel / 2, minSwordCount, maxSwordCount); // 根据玩家等级计算剑气数量
        float angleIncrement = (maxAngle - minAngle) / (swordCount - 1); // 计算角度增量

        if (swordCount == 1)
        {
            var th = _throwingsPool.Get();
            th.transform.position = _playerController.swordObject.transform.position;
            th.transform.forward = transform.forward;
            th.GetComponent<Rigidbody>().velocity = transform.forward * throwingsBehavior.throwingSpeed;
        }
        else
        {
            for (int i = 0; i < swordCount; i++)
            {
                var th = _throwingsPool.Get();
                th.transform.position = _playerController.swordObject.transform.position;

                // 计算剑气的角度
                float angle = minAngle + i * angleIncrement - (maxAngle - minAngle) / 2f;
                Vector3 direction = Quaternion.Euler(0, angle, 0) * _playerController.transform.forward;
                th.transform.forward = direction;

                Rigidbody rb = th.GetComponent<Rigidbody>();
                rb.velocity = direction * throwingsBehavior.throwingSpeed;
            }
        }
    }

    protected IEnumerator StartThrow(){
        _playerController.GetAnimator().SetTrigger(animatorTriggerName = "Throw");
        _playerController.isCrouching = false;
        _playerController.rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(animationGap);
        var th = _throwingsPool.Get();
        th.transform.position = _playerController.swordObject.transform.position;
        th.transform.forward = transform.forward;
        th.GetComponent<Rigidbody>().velocity = transform.forward * throwingsBehavior.throwingSpeed;
    }

    protected void StartCasting()
    {
        if (isCasting) return;
        isCasting = true;
        castingCoroutine = StartCoroutine(ImmediateCastAimingLogic());
    }

    protected IEnumerator StopCasting()
    {
        SkillPreview.SetActive(false);
        if (!isCasting) yield break;
        if (canCast)
        {
            _playerController.GetAnimator().SetTrigger(animatorTriggerName);
            yield return new WaitForSeconds(animationGap);
            var th = _throwingsPool.Get();
            th.transform.position = hitTarget + generatingOffset;
            th.transform.rotation = transform.rotation;
        }
        isCasting = false;
    }

    protected IEnumerator ImmediateCastAimingLogic()
    {
        SkillPreview.SetActive(true);
        // var castTrans = _playerController
        //     ? _spellCast.spellingPartTransform.position
        //     : transform.position + Vector3.up * 0.5f;
        var castTrans = _playerController.mycamera.transform.position;
        while (isCasting)
        {
            if (Physics.Raycast(castTrans, PlayerController.Instance.mycamera.transform.forward,
                    out RaycastHit hit, castingDistance, castingLayer))
            {
                hitTarget = hit.point;
                SkillPreview.transform.position = hitTarget;
                Vector3 playerToHitVector = castTrans - hit.point;
                float triggerRange = throwingsBehavior.triggerRange;
                DrawCircle(triggerRange, validColor, playerToHitVector.normalized * 0.08f, 30);
                canCast = true;
            }
            else
            {
                // out of spelling range, draw red circle
                SkillPreview.transform.position = new Vector3(castTrans.x, 0.5f, castTrans.z);

                // Ground height
                float groundHeight = transform.position.y;

                // Calculate the radius, considering the Pythagorean Theorem
                float radius = Mathf.Sqrt(castingDistance * castingDistance - (castTrans.y - groundHeight) * (castTrans.y - groundHeight));

                DrawCircle(radius, invalidColor, Vector3.zero, 100);
                canCast = false;
            }
            yield return null;
            // yield return new WaitForSeconds(0.04f);
        }
    }
    
    
    
    private void DrawCircle(float radius, Color color, Vector3 offset, int segments = 100)
    {
        lineRenderer.startColor = color; // update color
        lineRenderer.endColor = color;

        lineRenderer.positionCount = segments + 1;
        lineRenderer.useWorldSpace = false;

        float deltaTheta = (2f * Mathf.PI) / segments;
        float theta = 0f;

        for (int i = 0; i < segments + 1; i++)
        {
            float x = radius * Mathf.Cos(theta);
            float z = radius * Mathf.Sin(theta);

            Vector3 pos = new Vector3(x, 0, z) + offset; // Apply offset vector to avoid being buried underneath ground
            lineRenderer.SetPosition(i, pos);

            theta += deltaTheta;
        }
    }
}