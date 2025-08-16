using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家属性类型
/// </summary>
public enum EPlayerAttributeType
{
    Social,     // 社交
    Patience,   // 耐心
    Wisdom,     // 智慧
    Charisma    // 魅力
}

/// <summary>
/// 单个属性数据
/// </summary>
[System.Serializable]
public class PlayerAttributeData
{
    [Header("基础信息")]
    public EPlayerAttributeType attributeType;
    public string attributeName;
    public string description;
    public Sprite icon;

    [Header("数值信息")]
    public int currentLevel = 1;
    public int maxLevel = 10;
    public float currentValue;
    public float baseValue;
    public float valuePerLevel;

    [Header("升级信息")]
    public int upgradeCost = 100;
    public float costMultiplier = 1.5f;
    public int totalUpgradeCost; // 已花费的总升级费用

    public PlayerAttributeData(EPlayerAttributeType type)
    {
        attributeType = type;
        SetupAttributeInfo();
        RecalculateValue();
    }

    /// <summary>
    /// 设置属性基础信息
    /// </summary>
    private void SetupAttributeInfo()
    {
        switch (attributeType)
        {
            case EPlayerAttributeType.Social:
                attributeName = "社交";
                description = "每回合摸牌数+{0}";
                baseValue = 0f;
                valuePerLevel = 1f;
                upgradeCost = 100;
                break;
            case EPlayerAttributeType.Patience:
                attributeName = "耐心";
                description = "保留{0}点能量到下回合";
                baseValue = 0f;
                valuePerLevel = 1f;
                upgradeCost = 150;
                break;
            case EPlayerAttributeType.Wisdom:
                attributeName = "智慧";
                description = "每回合能量恢复+{0}";
                baseValue = 0f;
                valuePerLevel = 1f;
                upgradeCost = 120;
                break;
            case EPlayerAttributeType.Charisma:
                attributeName = "魅力";
                description = "股市影响力+{0}%";
                baseValue = 0f;
                valuePerLevel = 10f; // 每级增加10%影响力
                upgradeCost = 200;
                break;
        }
        costMultiplier = 1.5f;
    }

    /// <summary>
    /// 重新计算当前值
    /// </summary>
    public void RecalculateValue()
    {
        currentValue = baseValue + (currentLevel - 1) * valuePerLevel;
    }

    /// <summary>
    /// 获取下一级的升级费用
    /// </summary>
    public int GetNextUpgradeCost()
    {
        if (currentLevel >= maxLevel) return -1;

        return Mathf.RoundToInt(upgradeCost * Mathf.Pow(costMultiplier, currentLevel - 1));
    }

    /// <summary>
    /// 升级属性
    /// </summary>
    public bool UpgradeAttribute(int availableMoney)
    {
        if (currentLevel >= maxLevel) return false;

        int cost = GetNextUpgradeCost();
        if (cost <= availableMoney)
        {
            currentLevel++;
            totalUpgradeCost += cost;
            RecalculateValue();
            return true;
        }
        return false;
    }

    /// <summary>
    /// 获取格式化的描述
    /// </summary>
    public string GetFormattedDescription()
    {
        return string.Format(description, currentValue);
    }

    /// <summary>
    /// 获取属性显示字符串
    /// </summary>
    public string GetDisplayString()
    {
        return $"{attributeName} Lv.{currentLevel} ({currentValue})";
    }

    /// <summary>
    /// 获取升级预览字符串
    /// </summary>
    public string GetUpgradePreviewString()
    {
        if (currentLevel >= maxLevel)
            return "已达到最大等级";

        float nextValue = baseValue + currentLevel * valuePerLevel;
        int cost = GetNextUpgradeCost();
        return $"升级到 Lv.{currentLevel + 1} ({nextValue}) - 费用: {cost}金币";
    }
}

/// <summary>
/// 玩家属性系统数据
/// </summary>
[System.Serializable]
public class PlayerAttributesData
{
    [Header("属性列表")]
    public List<PlayerAttributeData> attributes = new List<PlayerAttributeData>();

    [Header("统计信息")]
    public int totalAttributePoints;
    public int totalMoneySpent;

    public PlayerAttributesData()
    {
        InitializeAttributes();
    }

    /// <summary>
    /// 初始化所有属性
    /// </summary>
    private void InitializeAttributes()
    {
        attributes.Clear();

        foreach (EPlayerAttributeType attributeType in Enum.GetValues(typeof(EPlayerAttributeType)))
        {
            attributes.Add(new PlayerAttributeData(attributeType));
        }

        RecalculateStats();
    }

    /// <summary>
    /// 获取指定属性
    /// </summary>
    public PlayerAttributeData GetAttribute(EPlayerAttributeType type)
    {
        return attributes.Find(attr => attr.attributeType == type);
    }

    /// <summary>
    /// 获取属性值
    /// </summary>
    public float GetAttributeValue(EPlayerAttributeType type)
    {
        var attribute = GetAttribute(type);
        return attribute?.currentValue ?? 0f;
    }

    /// <summary>
    /// 升级属性
    /// </summary>
    public bool UpgradeAttribute(EPlayerAttributeType type, int availableMoney)
    {
        var attribute = GetAttribute(type);
        if (attribute != null && attribute.UpgradeAttribute(availableMoney))
        {
            RecalculateStats();
            return true;
        }
        return false;
    }

    /// <summary>
    /// 重新计算统计信息
    /// </summary>
    private void RecalculateStats()
    {
        totalAttributePoints = 0;
        totalMoneySpent = 0;

        foreach (var attribute in attributes)
        {
            totalAttributePoints += attribute.currentLevel - 1; // 减去初始等级1
            totalMoneySpent += attribute.totalUpgradeCost;
        }
    }

    /// <summary>
    /// 获取总属性加成描述
    /// </summary>
    public List<string> GetAllEffectDescriptions()
    {
        List<string> descriptions = new List<string>();

        foreach (var attribute in attributes)
        {
            if (attribute.currentValue > 0)
            {
                descriptions.Add($"{attribute.attributeName}: {attribute.GetFormattedDescription()}");
            }
        }

        return descriptions;
    }
}
