using System.Collections;
using Behavior;
using UnityEngine;
using UnityEngine.Pool;
using Utility;
using rotation = ParticleRibbon.Scripts.rotation;

namespace ItemSystem
{
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

        public void SetPool(ObjectPool<GameObject> pool)
        {
            ThisPool = pool;
        }

        public void actionOnGet(){
            gameObject.SetActive(true);
            GetComponent<rotation>().clickOn();
        }

        public void actionOnRelease(){
            transform.SetParent(null);
            used = false;
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
            // player = GameObject.Find("Player");
            player = PlayerController.Instance.gameObject;
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
                GetComponent<rotation>().clickOff();
                StartCoroutine(ReleaseDelayed(0.6f));
            }
        }

        private IEnumerator ReleaseDelayed(float t){
            yield return new WaitForSeconds(t);
            Release();
        }

        public void Pick(){
            PickAction();
        }

        private void PickAction(){
            transform.position = PlayerController.Instance.pickHandTransform.position;
            transform.SetParent(PlayerController.Instance.pickHandTransform);
            if (_isEffNotNull)
            {
                used = true;
                _eff.AffectPlayer(player);
            }
            

            // 标记物体为销毁，Unity 会在下一帧销毁它
        }
    }
}
