using System;
using System.Collections.Generic;
using AttributeRelatedScript;
using CodeMonkey.HealthSystemCM;
using UI;
using Unity.VisualScripting;
using UnityEngine;

public class Sword : MonoBehaviour
{
    private PlayerController pCtrl;
    private Animator animator;
    [SerializeField] private BoxCollider swordCollider;
    private HashSet<Collider> hitEnemies = new HashSet<Collider>();

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
    }

    private void Update()
    {
        if (!animator) animator = pCtrl.GetAnimator();
    }

    private void OnTriggerEnter(Collider enemyCollider)
    {
        if (!animator.GetBool("isAttacking")) return;
        if (enemyCollider.CompareTag("Enemy") && !hitEnemies.Contains(enemyCollider))
        {
            // 剑碰到敌人时执行的操作
            // UI.UIManager.Instance.ShowMessage2("Taste My Sword !!!(While a little stupid)");

            HealthSystem healthSystem = enemyCollider.GetComponent<HealthSystemComponent>().GetHealthSystem();
            if (healthSystem != null)
            {
                var dmg = pCtrl.GetDamage();
                UIManager.Instance.ShowMessage1("A " + dmg + " Cut~");
                healthSystem.Damage(dmg); // Inflict damage on enemies
                hitEnemies.Add(enemyCollider); // 记录已攻击过的敌人
            }
        }
    }

    // 当攻击结束时清空哈希表
    private void HandleAttackEnded()
    {
        UIManager.Instance.ShowMessage2("End了");
        animator.SetBool("isAttacking", false);
        hitEnemies.Clear();
    }

    
}