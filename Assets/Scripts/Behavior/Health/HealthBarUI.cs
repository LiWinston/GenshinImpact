//Originally provided by Code Monkey in the Unity asset store, refactored by Yongchun Li 
//to add smooth transition coroutines and gradient color representation.

using System;
using System.Collections;
using CodeMonkey.HealthSystemCM;
using UnityEngine;
using UnityEngine.UI;

namespace enemyBehaviour.Health {

    public class HealthBarUI : MonoBehaviour {

        [SerializeField] private GameObject getHealthSystemGameObject;
        [SerializeField] private Image image;
        [SerializeField] private float fillSpeed = 5f; // 调整这个值以控制填充速度

        private HealthSystem healthSystem;
        private float targetFillAmount;
        private Coroutine changeFillCoroutine;

        private void Start() {
            if (HealthSystem.TryGetHealthSystem(getHealthSystemGameObject, out HealthSystem healthSystem)) {
                SetHealthSystem(healthSystem);
            }
        }

        public void SetHealthSystem(HealthSystem healthSystem) {
            if (this.healthSystem != null) {
                this.healthSystem.OnHealthChanged -= HealthSystem_OnHealthChanged;
            }
            this.healthSystem = healthSystem;

            UpdateHealthBarInstantly();

            healthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
            this.healthSystem.OnSetFull += HealthSystem_OnSetFull;
        }

        private void HealthSystem_OnSetFull(object sender, EventArgs e)
        {
            UpdateHealthBarInstantly();
        }

        private void HealthSystem_OnHealthChanged(object sender, System.EventArgs e) {
            // 当血量变化时，设置目标填充值
            targetFillAmount = healthSystem.GetHealthNormalized();
            // 如果之前有正在进行的缓动效果，停止它
            if (changeFillCoroutine != null) {
                StopCoroutine(changeFillCoroutine);
            }
            // 启动新的缓动效果
            changeFillCoroutine = StartCoroutine(ChangeFillSmoothly());
        }

        private void UpdateHealthBarInstantly() {
            float healthNormalized = healthSystem.GetHealthNormalized();
            image.fillAmount = healthNormalized;
            image.color = Color.Lerp(Color.red, Color.yellow, healthNormalized * 2);
            image.color = Color.Lerp(image.color, Color.green, healthNormalized * 2 - 1);
        }

        private IEnumerator ChangeFillSmoothly() {
            while (image.fillAmount != targetFillAmount) {
                image.fillAmount = Mathf.Lerp(image.fillAmount, targetFillAmount, Time.deltaTime * fillSpeed);
                
                float healthNormalized = image.fillAmount;
                Color color = Color.Lerp(Color.red, Color.yellow, healthNormalized * 2);
                color = Color.Lerp(color, Color.green, healthNormalized * 2 - 1);
                image.color = color;

                yield return null;
            }
            changeFillCoroutine = null;
        }


        private void OnDestroy() {
            healthSystem.OnHealthChanged -= HealthSystem_OnHealthChanged;
        }
    }
}
