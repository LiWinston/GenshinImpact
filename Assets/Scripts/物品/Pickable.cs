using CameraView;
using UnityEngine;

public class Pickable : MonoBehaviour
{
    public float pickupRange = 3.0f;
    public float alertDis = 2.0f; // 警告距离
    private bool used = false;
    private PlayerBuffEffect _eff;
    private bool _isEffNotNull;
    private GameObject player;
    private bool isPlayerInSight = false; // 玩家是否在视野内
    private bool showAlert = false; // 是否显示警告

    private void Start()
    {
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
            Destroy(gameObject);
        }

        // 检测玩家是否在视野内
        // showAlert = InSightDetector.IsInLineOfSight(player, this);

        // 根据条件显示警告
        if (Vector3.Distance(transform.position, player.transform.position) <= alertDis)
        {
            showAlert = true;
            player.GetComponent<PlayerController>().showExp("EEE");
        }
        else
        {
            showAlert = false;
        }
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
