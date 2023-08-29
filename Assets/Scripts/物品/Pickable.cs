using System;
using Unity.VisualScripting;
using UnityEngine;

public class Pickable : MonoBehaviour
{
    public float pickupRange = 3.0f;
    private bool used = false;
    private Effect _eff;
    private bool _isEffNotNull;
    private GameObject player;

    private void Awake()
    {
        _isEffNotNull = _eff != null;
    }

    private void Start()
    {
        _eff = GetComponent<Effect>();
        player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            // 找到了带有 "Player" 标签的游戏对象
            // 可以对该对象执行操作
        }
        else
        {
            // 没有找到带有 "Player" 标签的游戏对象
            // 可以处理找不到的情况
        }

    }

    public virtual void Pick()
    {
        if (_isEffNotNull)
        {
            _eff.AffectPlayer(player);
            used = true;
        }
    }

    public void LateUpdate()
    {
        if(used) Destroy(gameObject);
    }
}