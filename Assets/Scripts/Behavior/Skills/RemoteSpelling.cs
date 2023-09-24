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
    

    [InspectorLabel("对象池--ObjectPool")]
    [SerializeField]private int defaultCapacity = 10;
    [SerializeField]private int maxCapacity = 10;
    private ObjectPool<GameObject> _throwingsPool;
    public int countAll;
    public int countActive;
    public int countInactive;
    
    [InspectorLabel("Trying Cast Position")]
    private bool isCasting;
    private Coroutine castingCoroutine;
    public float castingDistance = 20f; // 施法最大距离

    private void Start()
    {
        _throwingsPool = new ObjectPool<GameObject>(CreateFunc, actionOnGet, actionOnRelease, actionOnDestroy,
            true, defaultCapacity, maxCapacity);
        castingLayer = LayerMask.GetMask("Wall","Floor");
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
        isCasting = false;
        if (castingCoroutine != null)
        {
            StopCoroutine(castingCoroutine);
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    protected IEnumerator CastingLogic(){
        var playerController = GetComponent<PlayerController>();
        var castTrans = playerController
            ? GetComponent<SpellCast>().spellingPartTransform.position
            : transform.position + Vector3.up * 0.5f;
        while (isCasting)
        {
            if (Physics.Raycast(castTrans, PlayerController.Instance.mycamera.transform.forward, 
                    out RaycastHit hit, castingDistance, castingLayer))
            {
                // 在碰撞点生成技能
                var th = _throwingsPool.Get();
                th.transform.position = hit.point + Vector3.up * 0.5f;
            }

            // 等待一段时间再进行下一次尝试
            yield return new WaitForSeconds(0.1f);
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