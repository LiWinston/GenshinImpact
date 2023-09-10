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
    private GameObject player;
    private Damage damage;
    private void Start()
    {
        player = GameObject.Find("Player");
        if (player == null)
        {
            Debug.LogError("Player for sword not Found!");
        }
        else
        {
            damage = player.GetComponent<Damage>();
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
                var dmg = damage.CurrentDamage;
                if (!player.GetComponent<State>().IsInCombat())
                {
                    dmg *= 2;
                }
                UIManager.Instance.ShowMessage1("A "+ dmg +" Cut~");
                healthSystem.Damage(damage.CurrentDamage); // Inflict damage on enemies
            }
        }
    }
}
