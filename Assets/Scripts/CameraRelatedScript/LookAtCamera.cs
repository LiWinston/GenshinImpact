using Behavior;
using UnityEngine;

namespace CameraRelatedScript {

    /// <summary>
    /// Utility script to make a Transform look straight at the main camera
    /// Useful for HealthBar World Objects, always face camera
    /// </summary>
    ///
    /// Authored by CodeMonkey, camera object Refined by yongchunLi
    public class LookAtCamera : MonoBehaviour {

        [SerializeField] private bool invert;
        // private Transform mainCameraTransform;

        private void Update() {
            LookAt();
        }

        private void OnEnable() {
            LookAt();
        }

        private void LookAt() {
            if (invert) {
                Vector3 dir = (transform.position - PlayerController.Instance.mycamera.transform.position).normalized;
                transform.LookAt(transform.position + dir);
            } else {
                transform.LookAt(PlayerController.Instance.mycamera.transform.position);
            }
        }

    }

}