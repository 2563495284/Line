using System;
using System.Collections.Generic;
using UnityEngine;
using SerializeReferenceEditor;

/// <summary>
/// 条件消耗效果 - 在执行主效果前检查并消耗资源
/// </summary>
[System.Serializable]
public class ConditionalCostEffect : Effect2
{

    [Header("消耗配置")]
    [SerializeField] private List<ResourceCost> resourceCosts = new List<ResourceCost>();

    [Header("成功后执行的效果")]
    [SerializeReference, SR] private List<Effect2> successEffects = new List<Effect2>();

    public override GameAction GetGameAction()
    {
        // 创建条件消耗的GameAction
        return new CheckAndConsumeResourceGA(resourceCosts, successEffects, characterView, targetLineView);
    }
}

/// <summary>
/// 资源消耗配置
/// </summary>
[System.Serializable]
public class ResourceCost
{
    public enum ResourceType
    {
        Stock,      // 股票
        Attribute   // 属性
    }

    [SerializeField] public ResourceType resourceType;

    // 股票相关
    [SerializeField] public EStockType stockType;
    [SerializeField] public bool markPoint = false;
    [SerializeField] public int stockAmount;

    // 属性相关
    [SerializeField] public EAttrType attributeType;
    [SerializeField] public float attributeAmount;

    /// <summary>
    /// 检查资源是否足够
    /// </summary>
    public bool CanAfford()
    {
        switch (resourceType)
        {
            case ResourceType.Stock:
                return MultiStockSystem.Ins.GetStockHoldings(stockType) >= stockAmount;
            case ResourceType.Attribute:
                return PlayerAttributeSystem.Ins.GetAttributeValue(attributeType) >= attributeAmount;
            default:
                return false;
        }
    }

    /// <summary>
    /// 消耗资源
    /// </summary>
    public GameAction GetConsumeAction()
    {
        switch (resourceType)
        {
            case ResourceType.Stock:
                return new ChangeStockGA(-stockAmount, stockType);
            case ResourceType.Attribute:
                return new ChangeAttributeGA(attributeType, -attributeAmount);
            default:
                return null;
        }
    }

    /// <summary>
    /// 获取资源描述
    /// </summary>
    public string GetDescription()
    {
        switch (resourceType)
        {
            case ResourceType.Stock:
                return $"{stockAmount}{GetStockName(stockType)}";
            case ResourceType.Attribute:
                return $"{attributeAmount}{GetAttributeName(attributeType)}";
            default:
                return "";
        }
    }

    private string GetStockName(EStockType type)
    {
        switch (type)
        {
            case EStockType.Oil: return "石油";
            case EStockType.Steel: return "钢铁";
            case EStockType.Cotton: return "棉花";
            default: return "";
        }
    }

    private string GetAttributeName(EAttrType type)
    {
        switch (type)
        {
            case EAttrType.Social: return "社交";
            case EAttrType.Wisdom: return "智慧";
            case EAttrType.Charisma: return "魅力";
            case EAttrType.Courage: return "勇气";
            case EAttrType.Calmness: return "冷静";
            case EAttrType.Fanaticism: return "狂热";
            default: return "";
        }
    }
}
