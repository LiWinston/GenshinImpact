using System;
using AttributeRelatedScript;
using UnityEngine;

public class Effect : MonoBehaviour
{
    public enum ItemType
    {
        heal,
        manaSupply,
        damageINC,
        //TO ADD CATAGORY
    }

    [SerializeField]private ItemType itemType;
    [SerializeField]private float effectValue = 40;
    
    // ReSharper disable Unity.PerformanceAnalysis
    public void AffectPlayer(GameObject obj)
    {
        if (!obj.CompareTag("Player"))
        {
            Console.WriteLine("Not Player in AffectPlayer(GameObject obj) in Effect.cs");
            return;
        }
        // 根据 itemType 实现不同的效果
        switch (itemType)
        {
            case ItemType.heal:
                var itemEffectStrategy1 = new HealItemEffect(effectValue);
                break;
            case ItemType.manaSupply:
                var itemEffectStrategy2 = new ManaSupplyItemEffect(effectValue);
                break;
            case ItemType.damageINC:
                var itemEffectStrategy3 = new DamageIncreaseItemEffect(effectValue);
                break;
            // 添加其他物品类型的效果策略
        }
    }
}