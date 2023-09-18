using System;
using UnityEngine;
namespace CodeMonkey.HealthSystemCM {

    /// <summary>
    /// Utility script to make a Transform look straight at the main camera
    /// Useful for HealthBar World Objects, always face camera
    /// </summary>
    ///
    /// Authored by CodeMonkey, camera object Refined by yongchunLi
    public class LookAtCamera : MonoBehaviour {

        [SerializeField] private bool invert;
        private PlayerController pCtrl;
        // private Transform mainCameraTransform;


        private void Start()
        {
            pCtrl = GameObject.Find("Player").GetComponent<PlayerController>();
            if (pCtrl == null)
            {
                Debug.LogError("Player controller for sword not found!");
            }
            // mainCameraTransform = pCtrl.mycamera.transform;
        }

        private void Update() {
            LookAt();
        }

        private void OnEnable() {
            LookAt();
        }

        private void LookAt() {
            if (invert) {
                Vector3 dir = (transform.position - pCtrl.mycamera.transform.position).normalized;
                transform.LookAt(transform.position + dir);
            } else {
                transform.LookAt(pCtrl.mycamera.transform.position);
            }
        }

    }

}