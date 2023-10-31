using System.Collections;
using System.Collections.Generic;
using Behavior.Health;
using UI;
using UnityEngine;

namespace Behavior
{
    public class Sword : MonoBehaviour
    {
        private PlayerController pCtrl;
        private Animator animator;
        [SerializeField] private BoxCollider swordCollider;
        private HashSet<Collider> hitEnemies = new HashSet<Collider>();
        private int enemyLayer;
        private bool hasDoneDieBehaviour = false;

        private void Start()
        {
            pCtrl = PlayerController.Instance;
            if (pCtrl == null)
            {
                Debug.LogError("Player controller for sword not found!");
            }

            // 订阅结束攻击事件
            pCtrl.OnAttackEnded += HandleAttackEnded;

            animator = pCtrl.GetAnimator();
            if (!swordCollider) swordCollider = GetComponent<BoxCollider>();
            swordCollider.enabled = true;

            // 获取敌人层级
            enemyLayer = LayerMask.NameToLayer("Enemy");
        }

        private void Update()
        {
            if (!animator) animator = pCtrl.GetAnimator();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!animator.GetBool("isAttacking") || other.gameObject.layer != enemyLayer || hitEnemies.Contains(other))
                return;

            HealthSystem healthSystem = other.GetComponent<HealthSystemComponent>()?.GetHealthSystem();
            if (healthSystem != null)
            {
                var dmg = pCtrl.GetDamage();
                UIManager.Instance.ShowMessage1("Made " + dmg + " Damage");
                healthSystem.Damage(dmg); // Inflict damage on enemies
                hitEnemies.Add(other); // 记录已攻击过的敌人
            }
        }

        private void HandleAttackEnded()
        {
            animator.SetBool("isAttacking", false);
            hitEnemies.Clear();
        }

        public void BehaviourOnHolderDie()
        {
            if(hasDoneDieBehaviour) return;
            hasDoneDieBehaviour = true;
            StartCoroutine(SwordOffHand());

        }

        private IEnumerator SwordOffHand()
        {
            var demonicSword = transform.parent.gameObject;
            var swrb = demonicSword.GetComponent<Rigidbody>();
            
            yield return new WaitForSeconds(0.8f);
            
            demonicSword.transform.SetParent(null);
            swrb.isKinematic = false;
            swrb.useGravity = true;
            // swordCollider.providesContacts = true;
            swordCollider.isTrigger = false;
            GetComponent<CapsuleCollider>().enabled = true;
            // swrb.AddForce(swrb.velocity.normalized * (swrb.mass * 4f), ForceMode.Impulse);
            swrb.AddForce(Vector3.back * (-2f * (swrb.mass * 2f)), ForceMode.Impulse);
        }
        
    }
}