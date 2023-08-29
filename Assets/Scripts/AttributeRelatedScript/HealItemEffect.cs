using System;
using UnityEngine;

namespace AttributeRelatedScript
{
    public class HealItemEffect : IItemEffectStrategy
    {
        public HealItemEffect(float healAmount)
        {
            _healAmount = healAmount;
        }

        [SerializeField]
        private float _healAmount; // 添加用于指定恢复生命值的字段

        [SerializeField]
        private string _healMessage = "Your health increased by {0}, now {1}!"; // 添加用于指定消息的字段

        public void ApplyEffect(GameObject player)
        {
            // 在这里实现恢复生命值的逻辑
            player.GetComponent<Health>().Heal(_healAmount); // 使用字段来指定恢复的生命值数量
            float currentHealth = player.GetComponent<Health>().currentHealth; // 获取玩家当前的生命值
            string message = string.Format(_healMessage, _healAmount, currentHealth); // 格式化消息

            UI.UIManager.ShowMessage1(message); // 显示消息
        }
    }
}