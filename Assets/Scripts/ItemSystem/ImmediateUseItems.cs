using CameraView;
using UnityEngine;
using UnityEngine.Pool;

public class ImmediateUseItems : MonoBehaviour, IPoolable
{
    public float pickupRange = 3.0f;
    public float alertDis = 2.0f; // 警告距离
    private bool used = false;
    private PlayerBuffEffect _eff;
    private bool _isEffNotNull;
    private GameObject player;
    // private bool isPlayerInSight = false; // 玩家是否在视野内
    // private bool showAlert = false; // 是否显示警告
    // private rotation _rotation;
    

    public ObjectPool<GameObject> ThisPool { get; set; }
    public bool IsExisting { get; set; }

    public void SetPool(UnityEngine.Pool.ObjectPool<GameObject> pool)
    {
        ThisPool = pool;
    }
    

    public void actionOnRelease()
    {
        //TODO: implement Package Sys
        // if (_isEffNotNull) player.GetComponent<Package>().addToPackage(this.gameObject);
    }

    public void Release()
    {
        ThisPool.Release(gameObject);
    }


    private void Start()
    {
        // _rotation = GetComponent<rotation>();
        _eff = GetComponent<PlayerBuffEffect>();
        player = GameObject.Find("Player");
        if (player == null)
        {
            Debug.LogError("No Player found!");
        }

        // 在 Start 中初始化 _isEffNotNull
        _isEffNotNull = _eff != null;
    }

    public void Update()
    {
        if (used)
        {
            Release();
        }
    }

    public void Pick()
    {
        if (_isEffNotNull)
        {
            used = true;
            _eff.AffectPlayer(player);
        }

        // _rotation.clickOff();

        // 标记物体为销毁，Unity 会在下一帧销毁它
    }
}
