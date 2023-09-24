using System;
using System.Collections;
using System.Collections.Generic;
using enemyBehaviour;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

[System.Serializable]
public class RemoteSpelling: MonoBehaviour
{
    [InspectorLabel("生成--GenerationInfo")]
    public GameObject prefab;
    [SerializeField] private float maxExistTime = 10f;
    internal LayerMask castingLayer;
    [SerializeField] private Vector3 generatingOffset = Vector3.up * 0.4f;
    

    [InspectorLabel("对象池--ObjectPool")]
    [SerializeField]private int defaultCapacity = 10;
    [SerializeField]private int maxCapacity = 10;
    private ObjectPool<GameObject> _throwingsPool;
    public int countAll;
    public int countActive;
    public int countInactive;
    
    [InspectorLabel("Trying Cast Position")]
    [SerializeAs("预览技能对象")] public GameObject SkillPreview;
    private LineRenderer lineRenderer;
    private bool isCasting;
    private Coroutine castingCoroutine;
    public float castingDistance = 7f; // 施法最大距离
    private Vector3 hitTarget;
    private RemoteThrowingsBehavior throwingsBehavior;
    private PlayerController _playerController;
    private SpellCast _spellCast;


    private void Start()
    {
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
        Destroy(obj);
    }
    
    private void Update()
    {
        
        countAll = _throwingsPool.CountAll;
        countActive = _throwingsPool.CountActive;
        countInactive = _throwingsPool.CountInactive;
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            StartCasting();
        }
        else if (Input.GetKeyUp(KeyCode.F))
        {
            StopCasting();
        }
    }
    
    protected void StartCasting()
    {
        if (isCasting) return;
        isCasting = true;
        castingCoroutine = StartCoroutine(CastingLogic());
    }

    protected void StopCasting()
    {
        if (!isCasting) return;
        var th = _throwingsPool.Get();
        th.transform.position = hitTarget + generatingOffset;
        StartCoroutine(ReturnToPoolDelayed(th, 2));
        isCasting = false;
        if (castingCoroutine != null)
        {
            StopCoroutine(castingCoroutine);
        }
        SkillPreview.SetActive(false);
    }

    
    protected IEnumerator CastingLogic()
    {
        SkillPreview.SetActive(true);
        var castTrans = _playerController
            ? _spellCast.spellingPartTransform.position
            : transform.position + Vector3.up * 0.5f;
        while (isCasting)
        {
            if (Physics.Raycast(castTrans, PlayerController.Instance.mycamera.transform.forward,
                    out RaycastHit hit, castingDistance, castingLayer))
            {
                if (hit.collider != null)
                {
                    // 命中物体，绘制绿色圆圈
                    hitTarget = hit.point;
                    SkillPreview.transform.position = hitTarget;
                    Vector3 playerToHitVector = castTrans - hit.point;
                    float aoeRange = throwingsBehavior.AOERange;
                    DrawCircle(aoeRange, Color.green, playerToHitVector.normalized * 0.1f, 30);
                }
            }
            else
            {
                // 未命中物体，绘制红色圆圈
                SkillPreview.transform.position = castTrans;
                DrawCircle(castingDistance, Color.red, Vector3.zero);
            }

            yield return new WaitForSeconds(0.04f);
        }
    }
    
    private void DrawCircle(float radius, Color color, Vector3 offset, int segments = 100)
    {
        lineRenderer.startColor = color; // 更新颜色
        lineRenderer.endColor = color;

        lineRenderer.positionCount = segments + 1;
        lineRenderer.useWorldSpace = false;

        float deltaTheta = (2f * Mathf.PI) / segments;
        float theta = 0f;

        for (int i = 0; i < segments + 1; i++)
        {
            float x = radius * Mathf.Cos(theta);
            float z = radius * Mathf.Sin(theta);

            Vector3 pos = new Vector3(x, 0, z) + offset; // 应用偏移向量
            lineRenderer.SetPosition(i, pos);

            theta += deltaTheta;
        }
    }

    
    private IEnumerator ReturnToPoolDelayed(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        // Return the object to the object pool
        if (obj.activeSelf)
        {
            _throwingsPool.Release(obj);
        }
    }
}