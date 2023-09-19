using System;
using System.Collections.Generic;
using AttributeRelatedScript;
using CodeMonkey.HealthSystemCM;
using enemyBehaviour.Health;
using UI;
using Unity.VisualScripting;
using UnityEngine;

public class Sword : MonoBehaviour
{
    private PlayerController pCtrl;
    private Animator animator;
    [SerializeField] private BoxCollider swordCollider;
    private HashSet<Collider> hitEnemies = new HashSet<Collider>();
    private int enemyLayer;

    private void Start()
    {
        pCtrl = GameObject.Find("Player").GetComponent<PlayerController>();
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
            UIManager.Instance.ShowMessage1("A " + dmg + " Cut~");
            healthSystem.Damage(dmg); // Inflict damage on enemies
            hitEnemies.Add(other); // 记录已攻击过的敌人
        }
    }

    private void HandleAttackEnded()
    {
        animator.SetBool("isAttacking", false);
        hitEnemies.Clear();
    }
}