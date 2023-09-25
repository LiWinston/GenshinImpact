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
public class 剑气: MonoBehaviour
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

    private void Start(){
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
        GameObject throwing = Instantiate(prefab, transform.position, Quaternion.identity);
        actionOnGet(throwing);//尝试新写法 解决不销毁的问题
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
                StartCoroutine(StartThrow());
            }
        }
    }

    protected IEnumerator StartThrow()
    {
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
}