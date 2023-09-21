using System;
using AttributeRelatedScript;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerBuffEffect : MonoBehaviour
{
    public enum EffectType
    {
        Health,
        Energy,
        Attack_Damage,
        //TO ADD CATAGORY
    }

    [FormerlySerializedAs("itemType")] [SerializeField]private EffectType effectType;
    [SerializeField]private float effectValue = 40;
    
    // ReSharper disable Unity.PerformanceAnalysis
    public void AffectPlayer(GameObject obj)
    {
        // Debug.Log("AffectPlayer() " + obj + " 被调用");
        if (!obj.CompareTag("Player"))
        {
            Console.WriteLine("Not Player in AffectPlayer(GameObject obj) in Effect.cs");
            return;
        }
        // 根据 itemType 实现不同的效果
        switch (effectType)
        {
            case EffectType.Health:
                var itemEffectStrategy1 = new HealItemEffect(effectType,effectValue);
                itemEffectStrategy1.ApplyEffect(obj);
                break;
            case EffectType.Energy:
                var itemEffectStrategy2 = new ManaSupplyItemEffect(effectType,effectValue);
                itemEffectStrategy2.ApplyEffect(obj);
                break;
            case EffectType.Attack_Damage:
                var itemEffectStrategy3 = new DamageIncreaseItemEffect(effectType,effectValue);
                itemEffectStrategy3.ApplyEffect(obj);
                break;
            // 添加其他物品类型的效果策略
        }
    }
}