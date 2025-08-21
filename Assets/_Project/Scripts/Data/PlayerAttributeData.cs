using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 玩家属性类型
/// </summary>
public enum EPlayerAttributeType
{
    Social,     // 社交
    Patience,   // 耐心
    Wisdom,     // 智慧
    Charisma,    // 魅力
    Courage,     // 勇气
    Calmness,     // 冷静
    Fanaticism     // 狂热
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
    public float currentValue;
    public PlayerAttributeData(EPlayerAttributeType type)
    {
        attributeType = type;
        SetupAttributeInfo();
    }

    /// <summary>
    /// 设置属性基础信息
    /// </summary>
    private void SetupAttributeInfo()
    {
        switch (attributeType)
        {
            case EPlayerAttributeType.Social://基础的
                attributeName = "社交";
                description = "每回合摸牌数+1，每回合失去1";
                currentValue = 0f;
                break;
            case EPlayerAttributeType.Patience://基础的
                attributeName = "耐心";
                description = "保留1点能量到下回合，每回合失去1";
                currentValue = 0f;
                break;
            case EPlayerAttributeType.Wisdom://中级
                attributeName = "智慧";
                description = "每回合能量恢复+1";
                currentValue = 0f;
                break;
            case EPlayerAttributeType.Charisma://中级
                attributeName = "魅力";
                description = "市场影响力+10%";
                currentValue = 0f;
                break;
            case EPlayerAttributeType.Courage://基础
                attributeName = "勇气";
                description = "市场交易数量+10%";
                currentValue = 0f;
                break;
            case EPlayerAttributeType.Calmness://中性
                attributeName = "冷静";
                description = "环境对市场价格影响-10%";
                currentValue = 0f;
                break;
            case EPlayerAttributeType.Fanaticism://中性
                attributeName = "狂热";
                description = "环境对市场价格影响+10%";
                currentValue = 0f;
                break;
        }
    }


    /// <summary>
    /// 获取格式化的描述
    /// </summary>
    public string GetFormattedDescription()
    {
        return description;
    }

    /// <summary>
    /// 获取属性显示字符串
    /// </summary>
    public string GetDisplayString()
    {
        return $"{attributeName} ({currentValue})";
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
