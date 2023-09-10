using System;
using System.Collections;
using System.Collections.Generic;
using AttributeRelatedScript;
using CodeMonkey.HealthSystemCM;
using UI;
using UnityEngine;
using Object = UnityEngine.Object;

// 剑的脚本
public class Sword : MonoBehaviour
{
    private PlayerController pCtrl;
    private void Start()
    {
        pCtrl = GameObject.Find("Player").GetComponent<PlayerController>();
        if (pCtrl == null)
        {
            Debug.LogError("Player ctrler for sword not Found!");
        }
    }

    private void OnTriggerEnter(Collider enemyCollider)
    {
        if (enemyCollider.CompareTag("Enemy"))
        {
            // 剑碰到敌人时执行的操作
            UI.UIManager.Instance.ShowMessage2("Taste My Sword !!!(While a little stupid)");
            HealthSystem healthSystem = enemyCollider.GetComponent<HealthSystemComponent>().GetHealthSystem();
            if (healthSystem != null)
            {
                
                var dmg = pCtrl.GetDamage();
                UIManager.Instance.ShowMessage1("A "+ dmg +" Cut~");
                healthSystem.Damage(dmg); // Inflict damage on enemies
            }
        }
    }
}
