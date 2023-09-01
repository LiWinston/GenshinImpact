using System;
using UI;
using UnityEngine;

public class Pickable : MonoBehaviour
{
    public float pickupRange = 3.0f;
    private bool used = false;
    private PlayerBuffEffect _eff;
    private bool _isEffNotNull;
    private GameObject player;

    private void Start()
    {
        _eff = GetComponent<PlayerBuffEffect>();
        player = GameObject.Find("Player");
        if(player == null) UIManager.ShowMessage1("NO Player found!");

        // 在 Start 中初始化 _isEffNotNull
        _isEffNotNull = _eff != null;
    }

    public void Update()
    {
        if(used) Destroy(gameObject);
    }

    public void Pick()
    {
        if (_isEffNotNull)
        {
            used = true;
            _eff.AffectPlayer(player);
            
        }

        // 标记物体为销毁，Unity 会在下一帧销毁它
    }
}